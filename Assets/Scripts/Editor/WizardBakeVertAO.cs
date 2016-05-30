using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WizardBakeVertAO : ScriptableWizard
{
	
	
	public int   samples = 512;
	public float maxRange = 1.5f;
	public float minRange = 0.0000000001f;
	public float intensity = 2.0f;
	public bool  resetExistingAlpha = true;
	public bool  averageColors = false;
	public bool  averageNormals = false;
	
	
	
	[MenuItem ("GameObject/Bake AO to Vertex Alpha")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<WizardBakeVertAO>("Bake AO to Vertex Alpha", "Create");
		

		EditorUtility.ClearProgressBar();
	}
	
	
	
	void OnWizardCreate ()
	{
		
		GameObject subject = Selection.activeGameObject;
		
		// Utility
		RaycastHit hit = new RaycastHit();
		
		GameObject debugSphere = GameObject.Find("p_pointsample");
		
		bool isDebug = ( debugSphere == null ) ? false : true;
		
		GameObject points       = null;
		GameObject psphere      = null;
		GameObject psphere_miss = null;
		GameObject psphere_hit  = null;
		

		if (isDebug) {
			
			points = GameObject.Find("points");
			
			if (points != null)
				GameObject.DestroyImmediate( points );
			
			points = new GameObject("points");
			
			psphere      = (GameObject) AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/p_pointsample.prefab",      typeof(GameObject) );
			psphere_miss = (GameObject) AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/p_pointsample_miss.prefab", typeof(GameObject) );
			psphere_hit  = (GameObject) AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/p_pointsample_hit.prefab",  typeof(GameObject) );
		}
		

		MeshFilter[] mfs = subject.GetComponentsInChildren<MeshFilter>();
		

		int sample = 0;
		int numVerts = 0;
		foreach (MeshFilter mf in mfs)
			numVerts += mf.sharedMesh.vertices.Length;
		int numSamples = numVerts * samples;
		

		foreach (MeshFilter mf in mfs) {
			
			Mesh mesh = mf.sharedMesh;
			

			Vector3[] verts = mesh.vertices;
			
			Color[] colors = mesh.colors;
			if (colors.Length==0) {
				Debug.Log("Mesh is missing color data.  Supplying...");
				colors = new Color[ verts.Length ];
			}
			
			if (resetExistingAlpha) {
				for( int ic=0; ic<colors.Length; ic++ )
					colors[ ic ].a = 1;
			}
			
			// Store normals
			Vector3[] normals = new Vector3[ mesh.normals.Length ];
			if (normals.Length==0)
				mesh.RecalculateNormals();
			
			if (averageNormals) {
				
				Mesh clonemesh = new Mesh();
				clonemesh.vertices = mesh.vertices;
				clonemesh.normals = mesh.normals;
				clonemesh.tangents = mesh.tangents;
				clonemesh.triangles = mesh.triangles;
				clonemesh.RecalculateBounds();
				clonemesh.RecalculateNormals();
				normals = clonemesh.normals;
				Object.DestroyImmediate(clonemesh);
				
			} else {
				
				// Otherwise, just use the originals
				normals = mesh.normals;
				
			}
			
			int i,j,l = 0;
			l = verts.Length;
			
			for (i=0; i<l; i++) {
				
				//object-space normal
				Vector3 nrm = normals[ i ];
				
				//vert in world space
				Vector3 v = subject.transform.TransformPoint( verts [ i ] );
				
				//offset in world space (displacement from vertex position)
				Vector3 n = subject.transform.TransformPoint( verts [ i ] + nrm );
				
				//world-space normal
				Vector3 wnrm = (n-v);
				wnrm.Normalize();
				
				//occlusion at this vertex
				float occ = 0;
				
				

				bool debugSample = false;

				if (debugSphere != null) {
					Vector3 testPos  = debugSphere.transform.position;
					float   testDist = debugSphere.GetComponent<MeshRenderer>().bounds.extents.x;
					Vector3 testNrm  = debugSphere.transform.forward;
					
					debugSample = ( Mathf.Abs( (testPos - v).magnitude ) <  testDist && Vector3.Dot( wnrm, testNrm ) > 0.6f ) ? true : false;
				}
				
				

				for (j=0; j<samples; j++) {
				
					float rot = 180.0f;
					float rot2 = rot / 2.0f;
					float rotx = (( rot * Random.value ) - rot2);
					float roty = (( rot * Random.value ) - rot2);
					float rotz = (( rot * Random.value ) - rot2);
					
					Vector3 dir = Quaternion.Euler( rotx, roty, rotz ) * Vector3.up;

					Quaternion dirq = Quaternion.FromToRotation(Vector3.up, wnrm);

					Vector3 ray = dirq * dir;

					Vector3 offset = Vector3.Reflect( ray, wnrm );
					
					// TEST
					bool isHit = false;

					// Raycast
					ray = ray * (maxRange/ray.magnitude);
					if ( Physics.Linecast( v-(offset*0.1f), v + ray, out hit ) ) {

						if ( hit.distance > minRange ) {

							occ += Mathf.Clamp01( 1 - ( hit.distance / maxRange ) );
							isHit = true;
							
						} 
						
					}
					

					if (debugSample) {
						GameObject sphere = null;
						
						if (isHit)
							sphere = (GameObject) EditorUtility.InstantiatePrefab( psphere_hit );
						else
							sphere = (GameObject) EditorUtility.InstantiatePrefab( psphere_miss );
						sphere.name += "_"+sample;
						Vector3 sp = ray;
						sp *= (maxRange / ray.magnitude);
						sphere.transform.position = v + sp;
						sphere.transform.parent = points.transform;
						
						sphere = (GameObject) EditorUtility.InstantiatePrefab( psphere );
						sphere.name += "_"+sample;
						sp = offset;
						sp *= (0.1f / offset.magnitude);
						sphere.transform.position = v - sp;
						sphere.transform.parent = points.transform;
					}
					
					// Update the progress bar periodically
					if (++sample % 500 == 0) {
						EditorUtility.DisplayProgressBar(
							"VERTEX AO",
							"Calculating...",
							(float)sample / (float)numSamples
						);
					}
					
					
				}
				

				occ = Mathf.Clamp01( 1 - ((occ*intensity)/samples) );

					colors[ i ].a = colors[ i ].a * occ;
				
			} 
			
			

			if (averageColors) {
				int[] tris = mesh.triangles;
				l = tris.Length;
				
				for (i=0; i<l; i+=3) {
					
					int vi0 = tris[ i+0 ];
					int vi1 = tris[ i+1 ];
					int vi2 = tris[ i+2 ];
					
					Color c0 = colors[ vi0 ];
					Color c1 = colors[ vi1 ];
					Color c2 = colors[ vi2 ];
					
					Color avg = new Color();
					avg.a = (c0.a + c1.a + c2.a) / 3;
					
					c0.a = c0.a + (avg.a - c0.a) / 2;
					c1.a = c1.a + (avg.a - c1.a) / 2;
					c2.a = c2.a + (avg.a - c2.a) / 2;
					
					colors[ vi0 ] = c0;
					colors[ vi1 ] = c1;
					colors[ vi2 ] = c2;
				}
			}
			
			
			mesh.colors = colors;
			
			
		} 
		
		
		EditorUtility.ClearProgressBar();
		
	}
	
	
	
	void OnWizardUpdate ()
	{
		helpString = "\nSelect an object, and bake away\n";
	} 
	
	
	

	void OnWizardOtherButton ()
	{	
	}
	
}
