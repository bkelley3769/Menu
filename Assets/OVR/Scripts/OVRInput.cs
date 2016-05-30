﻿/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OVRInput
{
	/// Identifies a Hand for querying the Position and Rotation of a given Hand Node.
	public enum Hand
	{
		Left = OVRPlugin.Node.LeftHand, ///< Identifies the Left Hand
		Right = OVRPlugin.Node.RightHand, ///< Identifies the Right Hand
	}

	[Flags]
	/// Virtual button mappings that allow the same input bindings to work across different controllers.
	public enum Button
	{
		None                      = 0,          ///< RawButton: [Xbox, Touch, LTouch, RTouch: None]
		One                       = 0x00000001, ///< RawButton: [Xbox, Touch, RTouch: A], [LTouch: X]
		Two                       = 0x00000002, ///< RawButton: [Xbox, Touch, RTouch: B], [LTouch: Y]
		Three                     = 0x00000004, ///< RawButton: [Xbox, Touch: X], [LTouch, RTouch: None]
		Four                      = 0x00000008, ///< RawButton: [Xbox, Touch: Y], [LTouch, RTouch: None]
		Start                     = 0x00000100, ///< RawButton: [Xbox: Start], [Touch, LTouch, RTouch: None]
		Back                      = 0x00000200, ///< RawButton: [Xbox: Back], [Touch, LTouch, RTouch: None]
		PrimaryShoulder           = 0x00001000, ///< RawButton: [Xbox: LShoulder], [Touch, LTouch, RTouch: None]
		PrimaryIndexTrigger       = 0x00002000, ///< RawButton: [Xbox, Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger]
		PrimaryHandTrigger        = 0x00004000, ///< RawButton: [Xbox: None], [Touch, LTouch: LHandTrigger], [RTouch: RHandTrigger]
		PrimaryThumbstick         = 0x00008000, ///< RawButton: [Xbox, Touch, LTouch: LThumbstick], [RTouch: RThumbstick]
		PrimaryThumbstickUp       = 0x00010000, ///< RawButton: [Xbox, Touch, LTouch: LThumbstickUp], [RTouch: RThumbstickUp]
		PrimaryThumbstickDown     = 0x00020000, ///< RawButton: [Xbox, Touch, LTouch: LThumbstickDown], [RTouch: RThumbstickDown]
		PrimaryThumbstickLeft     = 0x00040000, ///< RawButton: [Xbox, Touch, LTouch: LThumbstickLeft], [RTouch: RThumbstickLeft]
		PrimaryThumbstickRight    = 0x00080000, ///< RawButton: [Xbox, Touch, LTouch: LThumbstickRight], [RTouch: RThumbstickRight]
		SecondaryShoulder         = 0x00100000, ///< RawButton: [Xbox: RShoulder], [Touch, LTouch, RTouch: None]
		SecondaryIndexTrigger     = 0x00200000, ///< RawButton: [Xbox, Touch: RIndexTrigger], [LTouch, RTouch: None]
		SecondaryHandTrigger      = 0x00400000, ///< RawButton: [Xbox: None], [Touch: RHandTrigger], [LTouch, RTouch: None]
		SecondaryThumbstick       = 0x00800000, ///< RawButton: [Xbox, Touch: RThumbstick], [LTouch, RTouch: None]
		SecondaryThumbstickUp     = 0x01000000, ///< RawButton: [Xbox, Touch: RThumbstickUp], [LTouch, RTouch: None]
		SecondaryThumbstickDown   = 0x02000000, ///< RawButton: [Xbox, Touch: RThumbstickDown], [LTouch, RTouch: None]
		SecondaryThumbstickLeft   = 0x04000000, ///< RawButton: [Xbox, Touch: RThumbstickLeft], [LTouch, RTouch: None]
		SecondaryThumbstickRight  = 0x08000000, ///< RawButton: [Xbox, Touch: RThumbstickRight], [LTouch, RTouch: None]
		DpadUp                    = 0x00000010, ///< RawButton: [Xbox: DpadUp], [Touch, LTouch, RTouch: None]
		DpadDown                  = 0x00000020, ///< RawButton: [Xbox: DpadDown], [Touch, LTouch, RTouch: None]
		DpadLeft                  = 0x00000040, ///< RawButton: [Xbox: DpadLeft], [Touch, LTouch, RTouch: None]
		DpadRight                 = 0x00000080, ///< RawButton: [Xbox: DpadRight], [Touch, LTouch, RTouch: None]
		Up                        = 0x10000000, ///< RawButton: [Xbox: DpadUp], [Touch, LTouch: LThumbstickUp], [RTouch: RThumbstickUp]
		Down                      = 0x20000000, ///< RawButton: [Xbox: DpadDown], [Touch, LTouch: LThumbstickDown], [RTouch: RThumbstickDown]
		Left                      = 0x40000000, ///< RawButton: [Xbox: DpadLeft], [Touch, LTouch: LThumbstickLeft], [RTouch: RThumbstickLeft]
		Right     = unchecked((int)0x80000000), ///< RawButton: [Xbox: DpadRight], [Touch, LTouch: LThumbstickRight], [RTouch: RThumbstickRight]
		Any                       = ~None,      ///< RawButton: [Xbox, Touch, LTouch, RTouch: Any]
	}

	[Flags]
	/// Raw button mappings that can be used to directly query the state of a controller.
	public enum RawButton
	{
		None                      = 0,          ///< Physical Button: [Xbox, Touch, LTouch, RTouch: None]
		A                         = 0x00000001, ///< Physical Button: [Xbox, Touch, RTouch: A], [LTouch: None]
		B                         = 0x00000002, ///< Physical Button: [Xbox, Touch, RTouch: B], [LTouch: None]
		X                         = 0x00000100, ///< Physical Button: [Xbox, Touch, LTouch: X], [RTouch: None]
		Y                         = 0x00000200, ///< Physical Button: [Xbox, Touch, LTouch: Y], [RTouch: None]
		Start                     = 0x00100000, ///< Physical Button: [Xbox: Start], [Touch, LTouch, RTouch: None]
		Back                      = 0x00200000, ///< Physical Button: [Xbox: Back], [Touch, LTouch, RTouch: None]
		LShoulder                 = 0x00000800, ///< Physical Button: [Xbox: LShoulder], [Touch, LTouch, RTouch: None]
		LIndexTrigger             = 0x01000000, ///< Physical Button: [Xbox, Touch, LTouch: LIndexTrigger], [RTouch: None]
		LHandTrigger              = 0x02000000, ///< Physical Button: [Xbox: None], [Touch, LTouch: LHandTrigger], [RTouch: None]
		LThumbstick               = 0x00000400, ///< Physical Button: [Xbox, Touch, LTouch: LThumbstick], [RTouch: None]
		LThumbstickUp             = 0x00000010, ///< Physical Button: [Xbox, Touch, LTouch: LThumbstickUp], [RTouch: None]
		LThumbstickDown           = 0x00000020, ///< Physical Button: [Xbox, Touch, LTouch: LThumbstickDown], [RTouch: None]
		LThumbstickLeft           = 0x00000040, ///< Physical Button: [Xbox, Touch, LTouch: LThumbstickLeft], [RTouch: None]
		LThumbstickRight          = 0x00000080, ///< Physical Button: [Xbox, Touch, LTouch: LThumbstickRight], [RTouch: None]
		RShoulder                 = 0x00000008, ///< Physical Button: [Xbox: RShoulder], [Touch, LTouch, RTouch: None]
		RIndexTrigger             = 0x04000000, ///< Physical Button: [Xbox, Touch, RTouch: RIndexTrigger], [LTouch: None]
		RHandTrigger              = 0x08000000, ///< Physical Button: [Xbox: None], [Touch, RTouch: RHandTrigger], [LTouch: None]
		RThumbstick               = 0x00000004, ///< Physical Button: [Xbox, Touch, RTouch: RThumbstick], [LTouch: None]
		RThumbstickUp             = 0x00001000, ///< Physical Button: [Xbox, Touch, RTouch: RThumbstickUp], [LTouch: None]
		RThumbstickDown           = 0x00002000, ///< Physical Button: [Xbox, Touch, RTouch: RThumbstickDown], [LTouch: None]
		RThumbstickLeft           = 0x00004000, ///< Physical Button: [Xbox, Touch, RTouch: RThumbstickLeft], [LTouch: None]
		RThumbstickRight          = 0x00008000, ///< Physical Button: [Xbox, Touch, RTouch: RThumbstickRight], [LTouch: None]
		DpadUp                    = 0x00010000, ///< Physical Button: [Xbox: DpadUp], [Touch, LTouch, RTouch: None]
		DpadDown                  = 0x00020000, ///< Physical Button: [Xbox: DpadDown], [Touch, LTouch, RTouch: None]
		DpadLeft                  = 0x00040000, ///< Physical Button: [Xbox: DpadLeft], [Touch, LTouch, RTouch: None]
		DpadRight                 = 0x00080000, ///< Physical Button: [Xbox: DpadRight], [Touch, LTouch, RTouch: None]
		Any                       = ~None,      ///< Physical Button: [Xbox, Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Virtual capacitive touch mappings that allow the same input bindings to work across different controllers with capacitive touch support.
	public enum Touch
	{
		None                      = 0,                            ///< RawTouch: [Xbox, Touch, LTouch, RTouch: None]
		One                       = Button.One,                   ///< RawTouch: [Xbox: None], [Touch, RTouch: A], [LTouch: X]
		Two                       = Button.Two,                   ///< RawTouch: [Xbox: None], [Touch, RTouch: B], [LTouch: Y]
		Three                     = Button.Three,                 ///< RawTouch: [Xbox: None], [Touch: X], [LTouch, RTouch: None]
		Four                      = Button.Four,                  ///< RawTouch: [Xbox: None], [Touch: Y], [LTouch, RTouch: None]
		PrimaryIndexTrigger       = Button.PrimaryIndexTrigger,   ///< RawTouch: [Xbox: None], [Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger]
		PrimaryThumbstick         = Button.PrimaryThumbstick,     ///< RawTouch: [Xbox: None], [Touch, LTouch: LThumbstick], [RTouch: RThumbstick]
		SecondaryIndexTrigger     = Button.SecondaryIndexTrigger, ///< RawTouch: [Xbox: None], [Touch: RIndexTrigger], [LTouch, RTouch: None]
		SecondaryThumbstick       = Button.SecondaryThumbstick,   ///< RawTouch: [Xbox: None], [Touch: RThumbstick], [LTouch, RTouch: None]
		Any                       = ~None,                        ///< RawTouch: [Xbox: None], [Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Raw capacitive touch mappings that can be used to directly query the state of a controller.
	public enum RawTouch
	{
		None                      = 0,                            ///< Physical Touch: [Xbox, Touch, LTouch, RTouch: None]
		A                         = RawButton.A,                  ///< Physical Touch: [Xbox: None], [Touch, RTouch: A], [LTouch: None]
		B                         = RawButton.B,                  ///< Physical Touch: [Xbox: None], [Touch, RTouch: B], [LTouch: None]
		X                         = RawButton.X,                  ///< Physical Touch: [Xbox: None], [Touch, LTouch: X], [RTouch: None]
		Y                         = RawButton.Y,                  ///< Physical Touch: [Xbox: None], [Touch, LTouch: Y], [RTouch: None]
		LIndexTrigger             = 0x00001000,                   ///< Physical Touch: [Xbox: None], [Touch, LTouch: LIndexTrigger], [RTouch: None]
		LThumbstick               = RawButton.LThumbstick,        ///< Physical Touch: [Xbox: None], [Touch, LTouch: LThumbstick], [RTouch: None]
		RIndexTrigger             = 0x00000010,                   ///< Physical Touch: [Xbox: None], [Touch, RTouch: RIndexTrigger], [LTouch: None]
		RThumbstick               = RawButton.RThumbstick,        ///< Physical Touch: [Xbox: None], [Touch, RTouch: RThumbstick], [LTouch: None]
		Any                       = ~None,                        ///< Physical Touch: [Xbox: None], [Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Virtual near touch mappings that allow the same input bindings to work across different controllers with near touch support.
	/// A near touch uses the capacitive touch sensors of a controller to detect approximate finger proximity prior to a full touch being reported.
	public enum NearTouch
	{
		None                      = 0,          ///< RawNearTouch: [Xbox, Touch, LTouch, RTouch: None]
		PrimaryIndexTrigger       = 0x00000001, ///< RawNearTouch: [Xbox: None], [Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger]
		PrimaryThumbButtons       = 0x00000002, ///< RawNearTouch: [Xbox: None], [Touch, LTouch: LThumbButtons], [RTouch: RThumbButtons]
		SecondaryIndexTrigger     = 0x00000004, ///< RawNearTouch: [Xbox: None], [Touch: RIndexTrigger], [LTouch, RTouch: None]
		SecondaryThumbButtons     = 0x00000008, ///< RawNearTouch: [Xbox: None], [Touch: RThumbButtons], [LTouch, RTouch: None]
		Any                       = ~None,      ///< RawNearTouch: [Xbox: None], [Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Raw near touch mappings that can be used to directly query the state of a controller.
	public enum RawNearTouch
	{
		None                      = 0,          ///< Physical NearTouch: [Xbox, Touch, LTouch, RTouch: None]
		LIndexTrigger             = 0x00000001, ///< Physical NearTouch: [Xbox: None], Implies finger is in close proximity to LIndexTrigger.
		LThumbButtons             = 0x00000002, ///< Physical NearTouch: [Xbox: None], Implies thumb is in close proximity to LThumbstick OR X/Y buttons.
		RIndexTrigger             = 0x00000004, ///< Physical NearTouch: [Xbox: None], Implies finger is in close proximity to RIndexTrigger.
		RThumbButtons             = 0x00000008, ///< Physical NearTouch: [Xbox: None], Implies thumb is in close proximity to RThumbstick OR A/B buttons.
		Any                       = ~None,      ///< Physical NearTouch: [Xbox: None], [Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Virtual 1-dimensional axis (float) mappings that allow the same input bindings to work across different controllers.
	public enum Axis1D
	{
		None                      = 0,     ///< RawAxis1D: [Xbox, Touch, LTouch, RTouch: None]
		PrimaryIndexTrigger       = 0x01,  ///< RawAxis1D: [Xbox, Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger]
		PrimaryHandTrigger        = 0x04,  ///< RawAxis1D: [Xbox: None], [Touch, LTouch: LHandTrigger], [RTouch: RHandTrigger]
		SecondaryIndexTrigger     = 0x02,  ///< RawAxis1D: [Xbox, Touch: RIndexTrigger], [LTouch, RTouch: None]
		SecondaryHandTrigger      = 0x08,  ///< RawAxis1D: [Xbox: None], [Touch: RHandTrigger], [LTouch, RTouch: None]
		Any                       = ~None, ///< RawAxis1D: [Xbox, Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Raw 1-dimensional axis (float) mappings that can be used to directly query the state of a controller.
	public enum RawAxis1D
	{
		None                      = 0,     ///< Physical Axis1D: [Xbox, Touch, LTouch, RTouch: None]
		LIndexTrigger             = 0x01,  ///< Physical Axis1D: [Xbox, Touch, LTouch: LIndexTrigger], [RTouch: None]
		LHandTrigger              = 0x04,  ///< Physical Axis1D: [Xbox: None], [Touch, LTouch: LHandTrigger], [RTouch: None]
		RIndexTrigger             = 0x02,  ///< Physical Axis1D: [Xbox, Touch, RTouch: RIndexTrigger], [LTouch: None]
		RHandTrigger              = 0x08,  ///< Physical Axis1D: [Xbox: None], [Touch, RTouch: RHandTrigger], [LTouch: None]
		Any                       = ~None, ///< Physical Axis1D: [Xbox, Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Virtual 2-dimensional axis (Vector2) mappings that allow the same input bindings to work across different controllers.
	public enum Axis2D
	{
		None                      = 0,     ///< RawAxis2D: [Xbox, Touch, LTouch, RTouch: None]
		PrimaryThumbstick         = 0x01,  ///< RawAxis2D: [Xbox, Touch, LTouch: LThumbstick], [RTouch: RThumbstick]
		SecondaryThumbstick       = 0x02,  ///< RawAxis2D: [Xbox, Touch: RThumbstick], [LTouch, RTouch: None]
		Any                       = ~None, ///< RawAxis2D: [Xbox, Touch, LTouch, RTouch: Any]
	}

    [Flags]
	/// Raw 2-dimensional axis (Vector2) mappings that can be used to directly query the state of a controller.
	public enum RawAxis2D
	{
		None                      = 0,     ///< Physical Axis2D: [Xbox, Touch, LTouch, RTouch: None]
		LThumbstick               = 0x01,  ///< Physical Axis2D: [Xbox, Touch, LTouch: LThumbstick], [RTouch: None]
		RThumbstick               = 0x02,  ///< Physical Axis2D: [Xbox, Touch, RTouch: RThumbstick], [LTouch: None]
		Any                       = ~None, ///< Physical Axis2D: [Xbox, Touch, LTouch, RTouch: Any]
	}

	[Flags]
	/// Identifies a ControllerType which can be used to query the virtual or raw input state of a specific controller.
	public enum ControllerType
	{
		None                      = 0,                          ///< Null controller.
		LTouch                    = 0x00000001,                 ///< Left Oculus Touch controller. Virtual input mapping differs from the combined L/R Touch mapping.
		RTouch                    = 0x00000002,                 ///< Right Oculus Touch controller. Virtual input mapping differs from the combined L/R Touch mapping.
		Touch                     = LTouch | RTouch,            ///< Combined Left/Right pair of Oculus Touch controllers.
		Xbox                      = 0x00000008,                 ///< Xbox 360 or Xbox One gamepad controller.
		Active                    = unchecked((int)0x80000000), ///< Default controller. Represents the controller that most recently registered a button press from the user.
		All                       = ~None,                      ///< Represents the logical OR of all controllers.
	}

	private static readonly float AXIS_AS_BUTTON_THRESHOLD = 0.5f;
	private List<OVRControllerBase> controllers;
	private ControllerType activeControllerType = ControllerType.All;
	private ControllerType connectedControllerTypes = ControllerType.None;

	/// <summary>
	/// Creates an instance of OVRInput. Called by OVRManager.
	/// </summary>
	public OVRInput()
	{
		controllers = new List<OVRControllerBase>
		{
#if !UNITY_ANDROID || UNITY_EDITOR
			new OVRControllerXbox(),
			new OVRControllerTouch(),
			new OVRControllerLTouch(),
			new OVRControllerRTouch(),
#endif
		};
	}

	/// <summary>
	/// Updates the internal state of the OVRInput. Called by OVRManager.
	/// </summary>
	public void Update()
	{
		if (!OVRManager.instance.isVRPresent)
			return;

		connectedControllerTypes = ControllerType.None;

		for (int i = 0; i < controllers.Count; i++)
		{
			connectedControllerTypes |= controllers[i].Update();
			if (Get(RawButton.Any, controllers[i].controllerType))
			{
				activeControllerType = controllers[i].controllerType;
			}
		}

		if ((activeControllerType == ControllerType.LTouch) || (activeControllerType == ControllerType.RTouch))
		{
			// If either Touch controller is Active, set both to Active.
			activeControllerType = ControllerType.Touch;
		}

		if ((connectedControllerTypes & activeControllerType) == 0)
		{
			activeControllerType = ControllerType.None;
		}
	}

	/// <summary>
	/// Gets the local Hand position of the given Hand.
	/// </summary>
	public static Vector3 GetLocalHandPosition(OVRInput.Hand hand)
	{
		if (!OVRManager.instance.isVRPresent)
			return Vector3.zero;

		return OVRPlugin.GetNodePose((OVRPlugin.Node)hand+3).ToOVRPose().position;
	}

	/// <summary>
	/// Gets the local Hand rotation of the given Hand.
	/// </summary>
	public static Quaternion GetLocalHandRotation(OVRInput.Hand hand)
	{
		if (!OVRManager.instance.isVRPresent)
			return Quaternion.identity;

		return OVRPlugin.GetNodePose((OVRPlugin.Node)hand+3).ToOVRPose().orientation;
	}

	/// <summary>
	/// Gets the current state of the given virtual button mask on the Active controller.
	/// Returns true if any masked button is down on the Active controller.
	/// </summary>
	public static bool Get(Button virtualMask)
	{
		return OVRManager.input.GetResolvedButton(virtualMask, RawButton.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given virtual button mask with the given controller mask.
	/// Returns true if any masked button is down on any masked controller.
	/// </summary>
	public static bool Get(Button virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButton(virtualMask, RawButton.None, controllerMask);
	}

	/// <summary>
	/// Gets the current state of the given raw button mask with the Active controller.
	/// Returns true if any masked button is down on the Active controller.
	/// </summary>
	public static bool Get(RawButton rawMask)
	{
		return OVRManager.input.GetResolvedButton(Button.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given raw button mask with the given controller mask.
	/// Returns true if any masked button is down on any masked controllers.
	/// </summary>
	public static bool Get(RawButton rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButton(Button.None, rawMask, controllerMask);
	}

	private bool GetResolvedButton(Button virtualMask, RawButton rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.currentInputState.Buttons & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the current down state of the given virtual button mask with the Active controller.
	/// Returns true if any masked button was pressed this frame on the Active controller and no masked button was previously down last frame.
	/// </summary>
	public static bool GetDown(Button virtualMask)
	{
		return OVRManager.input.GetResolvedButtonDown(virtualMask, RawButton.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given virtual button mask with the given controller mask.
	/// Returns true if any masked button was pressed this frame on any masked controller and no masked button was previously down last frame.
	/// </summary>
	public static bool GetDown(Button virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButtonDown(virtualMask, RawButton.None, controllerMask);
	}

	/// <summary>
	/// Gets the current down state of the given raw button mask with the Active controller.
	/// Returns true if any masked button was pressed this frame on the Active controller and no masked button was previously down last frame.
	/// </summary>
	public static bool GetDown(RawButton rawMask)
	{
		return OVRManager.input.GetResolvedButtonDown(Button.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given raw button mask with the given controller mask.
	/// Returns true if any masked button was pressed this frame on any masked controller and no masked button was previously down last frame.
	/// </summary>
	public static bool GetDown(RawButton rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButtonDown(Button.None, rawMask, controllerMask);
	}

	private bool GetResolvedButtonDown(Button virtualMask, RawButton rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool down = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.previousInputState.Buttons & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawButton)controller.currentInputState.Buttons & resolvedMask) != 0)
					&& (((RawButton)controller.previousInputState.Buttons & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	/// <summary>
	/// Gets the current up state of the given virtual button mask with the Active controller.
	/// Returns true if any masked button was released this frame on the Active controller and no other masked button is still down this frame.
	/// </summary>
	public static bool GetUp(Button virtualMask)
	{
		return OVRManager.input.GetResolvedButtonUp(virtualMask, RawButton.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given virtual button mask with the given controller mask.
	/// Returns true if any masked button was released this frame on any masked controller and no other masked button is still down this frame.
	/// </summary>
	public static bool GetUp(Button virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButtonUp(virtualMask, RawButton.None, controllerMask);
	}

	/// <summary>
	/// Gets the current up state of the given raw button mask with the Active controller.
	/// Returns true if any masked button was released this frame on the Active controller and no other masked button is still down this frame.
	/// </summary>
	public static bool GetUp(RawButton rawMask)
	{
		return OVRManager.input.GetResolvedButtonUp(Button.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given raw button mask with the given controller mask.
	/// Returns true if any masked button was released this frame on any masked controller and no other masked button is still down this frame.
	/// </summary>
	public static bool GetUp(RawButton rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedButtonUp(Button.None, rawMask, controllerMask);
	}

	private bool GetResolvedButtonUp(Button virtualMask, RawButton rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool up = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.currentInputState.Buttons & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawButton)controller.currentInputState.Buttons & resolvedMask) == 0)
					&& (((RawButton)controller.previousInputState.Buttons & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	/// <summary>
	/// Gets the current state of the given virtual touch mask on the Active controller.
	/// Returns true if any masked touch is down on the Active controller.
	/// </summary>
	public static bool Get(Touch virtualMask)
	{
		return OVRManager.input.GetResolvedTouch(virtualMask, RawTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given virtual touch mask with the given controller mask.
	/// Returns true if any masked touch is down on any masked controller.
	/// </summary>
	public static bool Get(Touch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouch(virtualMask, RawTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current state of the given raw touch mask with the Active controller.
	/// Returns true if any masked touch is down on the Active controller.
	/// </summary>
	public static bool Get(RawTouch rawMask)
	{
		return OVRManager.input.GetResolvedTouch(Touch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given raw touch mask with the given controller mask.
	/// Returns true if any masked touch is down on any masked controllers.
	/// </summary>
	public static bool Get(RawTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouch(Touch.None, rawMask, controllerMask);
	}

	private bool GetResolvedTouch(Touch virtualMask, RawTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.currentInputState.Touches & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the current down state of the given virtual touch mask with the Active controller.
	/// Returns true if any masked touch was pressed this frame on the Active controller and no masked touch was previously down last frame.
	/// </summary>
	public static bool GetDown(Touch virtualMask)
	{
		return OVRManager.input.GetResolvedTouchDown(virtualMask, RawTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given virtual touch mask with the given controller mask.
	/// Returns true if any masked touch was pressed this frame on any masked controller and no masked touch was previously down last frame.
	/// </summary>
	public static bool GetDown(Touch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouchDown(virtualMask, RawTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current down state of the given raw touch mask with the Active controller.
	/// Returns true if any masked touch was pressed this frame on the Active controller and no masked touch was previously down last frame.
	/// </summary>
	public static bool GetDown(RawTouch rawMask)
	{
		return OVRManager.input.GetResolvedTouchDown(Touch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given raw touch mask with the given controller mask.
	/// Returns true if any masked touch was pressed this frame on any masked controller and no masked touch was previously down last frame.
	/// </summary>
	public static bool GetDown(RawTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouchDown(Touch.None, rawMask, controllerMask);
	}

	private bool GetResolvedTouchDown(Touch virtualMask, RawTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool down = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.previousInputState.Touches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawTouch)controller.currentInputState.Touches & resolvedMask) != 0)
					&& (((RawTouch)controller.previousInputState.Touches & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	/// <summary>
	/// Gets the current up state of the given virtual touch mask with the Active controller.
	/// Returns true if any masked touch was released this frame on the Active controller and no other masked touch is still down this frame.
	/// </summary>
	public static bool GetUp(Touch virtualMask)
	{
		return OVRManager.input.GetResolvedTouchUp(virtualMask, RawTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given virtual touch mask with the given controller mask.
	/// Returns true if any masked touch was released this frame on any masked controller and no other masked touch is still down this frame.
	/// </summary>
	public static bool GetUp(Touch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouchUp(virtualMask, RawTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current up state of the given raw touch mask with the Active controller.
	/// Returns true if any masked touch was released this frame on the Active controller and no other masked touch is still down this frame.
	/// </summary>
	public static bool GetUp(RawTouch rawMask)
	{
		return OVRManager.input.GetResolvedTouchUp(Touch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given raw touch mask with the given controller mask.
	/// Returns true if any masked touch was released this frame on any masked controller and no other masked touch is still down this frame.
	/// </summary>
	public static bool GetUp(RawTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedTouchUp(Touch.None, rawMask, controllerMask);
	}

	private bool GetResolvedTouchUp(Touch virtualMask, RawTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool up = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.currentInputState.Touches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawTouch)controller.currentInputState.Touches & resolvedMask) == 0)
					&& (((RawTouch)controller.previousInputState.Touches & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	/// <summary>
	/// Gets the current state of the given virtual near touch mask on the Active controller.
	/// Returns true if any masked near touch is down on the Active controller.
	/// </summary>
	public static bool Get(NearTouch virtualMask)
	{
		return OVRManager.input.GetResolvedNearTouch(virtualMask, RawNearTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given virtual near touch mask with the given controller mask.
	/// Returns true if any masked near touch is down on any masked controller.
	/// </summary>
	public static bool Get(NearTouch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouch(virtualMask, RawNearTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current state of the given raw near touch mask with the Active controller.
	/// Returns true if any masked near touch is down on the Active controller.
	/// </summary>
	public static bool Get(RawNearTouch rawMask)
	{
		return OVRManager.input.GetResolvedNearTouch(NearTouch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given raw near touch mask with the given controller mask.
	/// Returns true if any masked near touch is down on any masked controllers.
	/// </summary>
	public static bool Get(RawNearTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouch(NearTouch.None, rawMask, controllerMask);
	}

	private bool GetResolvedNearTouch(NearTouch virtualMask, RawNearTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.currentInputState.NearTouches & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the current down state of the given virtual near touch mask with the Active controller.
	/// Returns true if any masked near touch was pressed this frame on the Active controller and no masked near touch was previously down last frame.
	/// </summary>
	public static bool GetDown(NearTouch virtualMask)
	{
		return OVRManager.input.GetResolvedNearTouchDown(virtualMask, RawNearTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given virtual near touch mask with the given controller mask.
	/// Returns true if any masked near touch was pressed this frame on any masked controller and no masked near touch was previously down last frame.
	/// </summary>
	public static bool GetDown(NearTouch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouchDown(virtualMask, RawNearTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current down state of the given raw near touch mask with the Active controller.
	/// Returns true if any masked near touch was pressed this frame on the Active controller and no masked near touch was previously down last frame.
	/// </summary>
	public static bool GetDown(RawNearTouch rawMask)
	{
		return OVRManager.input.GetResolvedNearTouchDown(NearTouch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current down state of the given raw near touch mask with the given controller mask.
	/// Returns true if any masked near touch was pressed this frame on any masked controller and no masked near touch was previously down last frame.
	/// </summary>
	public static bool GetDown(RawNearTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouchDown(NearTouch.None, rawMask, controllerMask);
	}

	private bool GetResolvedNearTouchDown(NearTouch virtualMask, RawNearTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool down = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.previousInputState.NearTouches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawNearTouch)controller.currentInputState.NearTouches & resolvedMask) != 0)
					&& (((RawNearTouch)controller.previousInputState.NearTouches & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	/// <summary>
	/// Gets the current up state of the given virtual near touch mask with the Active controller.
	/// Returns true if any masked near touch was released this frame on the Active controller and no other masked near touch is still down this frame.
	/// </summary>
	public static bool GetUp(NearTouch virtualMask)
	{
		return OVRManager.input.GetResolvedNearTouchUp(virtualMask, RawNearTouch.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given virtual near touch mask with the given controller mask.
	/// Returns true if any masked near touch was released this frame on any masked controller and no other masked near touch is still down this frame.
	/// </summary>
	public static bool GetUp(NearTouch virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouchUp(virtualMask, RawNearTouch.None, controllerMask);
	}

	/// <summary>
	/// Gets the current up state of the given raw near touch mask with the Active controller.
	/// Returns true if any masked near touch was released this frame on the Active controller and no other masked near touch is still down this frame.
	/// </summary>
	public static bool GetUp(RawNearTouch rawMask)
	{
		return OVRManager.input.GetResolvedNearTouchUp(NearTouch.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current up state of the given raw near touch mask with the given controller mask.
	/// Returns true if any masked near touch was released this frame on any masked controller and no other masked near touch is still down this frame.
	/// </summary>
	public static bool GetUp(RawNearTouch rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedNearTouchUp(NearTouch.None, rawMask, controllerMask);
	}

	private bool GetResolvedNearTouchUp(NearTouch virtualMask, RawNearTouch rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return false;

		bool up = false;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.currentInputState.NearTouches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawNearTouch)controller.currentInputState.NearTouches & resolvedMask) == 0)
					&& (((RawNearTouch)controller.previousInputState.NearTouches & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	/// <summary>
	/// Gets the current state of the given virtual 1-dimensional axis mask on the Active controller.
	/// Returns the value of the largest masked axis on the Active controller. Values range from 0 to 1.
	/// </summary>
	public static float Get(Axis1D virtualMask)
	{
		return OVRManager.input.GetResolvedAxis1D(virtualMask, RawAxis1D.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given virtual 1-dimensional axis mask on the given controller mask.
	/// Returns the value of the largest masked axis across all masked controllers. Values range from 0 to 1.
	/// </summary>
	public static float Get(Axis1D virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedAxis1D(virtualMask, RawAxis1D.None, controllerMask);
	}

	/// <summary>
	/// Gets the current state of the given raw 1-dimensional axis mask on the Active controller.
	/// Returns the value of the largest masked axis on the Active controller. Values range from 0 to 1.
	/// </summary>
	public static float Get(RawAxis1D rawMask)
	{
		return OVRManager.input.GetResolvedAxis1D(Axis1D.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given raw 1-dimensional axis mask on the given controller mask.
	/// Returns the value of the largest masked axis across all masked controllers. Values range from 0 to 1.
	/// </summary>
	public static float Get(RawAxis1D rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedAxis1D(Axis1D.None, rawMask, controllerMask);
	}

	private float GetResolvedAxis1D(Axis1D virtualMask, RawAxis1D rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return 0.0f;

		float maxAxis = 0.0f;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			RawAxis1D resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				if ((RawAxis1D.LIndexTrigger & resolvedMask) != 0)
				{
					maxAxis = CalculateMax(maxAxis, controller.currentInputState.LIndexTrigger);
				}
				if ((RawAxis1D.RIndexTrigger & resolvedMask) != 0)
				{
					maxAxis = CalculateMax(maxAxis, controller.currentInputState.RIndexTrigger);
				}
				if ((RawAxis1D.LHandTrigger & resolvedMask) != 0)
				{
					maxAxis = CalculateMax(maxAxis, controller.currentInputState.LHandTrigger);
				}
				if ((RawAxis1D.RHandTrigger & resolvedMask) != 0)
				{
					maxAxis = CalculateMax(maxAxis, controller.currentInputState.RHandTrigger);
				}
			}
		}

		return maxAxis;
	}

	/// <summary>
	/// Gets the current state of the given virtual 2-dimensional axis mask on the Active controller.
	/// Returns the vector of the largest masked axis on the Active controller. Values range from -1 to 1.
	/// </summary>
	public static Vector2 Get(Axis2D virtualMask)
	{
		return OVRManager.input.GetResolvedAxis2D(virtualMask, RawAxis2D.None, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given virtual 2-dimensional axis mask on the given controller mask.
	/// Returns the vector of the largest masked axis across all masked controllers. Values range from -1 to 1.
	/// </summary>
	public static Vector2 Get(Axis2D virtualMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedAxis2D(virtualMask, RawAxis2D.None, controllerMask);
	}

	/// <summary>
	/// Gets the current state of the given raw 2-dimensional axis mask on the Active controller.
	/// Returns the vector of the largest masked axis on the Active controller. Values range from -1 to 1.
	/// </summary>
	public static Vector2 Get(RawAxis2D rawMask)
	{
		return OVRManager.input.GetResolvedAxis2D(Axis2D.None, rawMask, ControllerType.Active);
	}

	/// <summary>
	/// Gets the current state of the given raw 2-dimensional axis mask on the given controller mask.
	/// Returns the vector of the largest masked axis across all masked controllers. Values range from -1 to 1.
	/// </summary>
	public static Vector2 Get(RawAxis2D rawMask, ControllerType controllerMask)
	{
		return OVRManager.input.GetResolvedAxis2D(Axis2D.None, rawMask, controllerMask);
	}

	private Vector2 GetResolvedAxis2D(Axis2D virtualMask, RawAxis2D rawMask, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return Vector2.zero;

		Vector2 maxAxis = Vector2.zero;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			RawAxis2D resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				if ((RawAxis2D.LThumbstick & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentInputState.LThumbstick.x,
						controller.currentInputState.LThumbstick.y);

					maxAxis = CalculateMax(maxAxis, axis);
				}
				if ((RawAxis2D.RThumbstick & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentInputState.RThumbstick.x,
						controller.currentInputState.RThumbstick.y);

					maxAxis = CalculateMax(maxAxis, axis);
				}
			}
		}

		return maxAxis;
	}

	/// <summary>
	/// Returns a mask of all currently connected controller types.
	/// </summary>
	public static ControllerType GetConnectedControllerTypes()
	{
		if (!OVRManager.instance.isVRPresent)
			return ControllerType.None;

		return OVRManager.input.connectedControllerTypes;
	}

	/// <summary>
	/// Returns the current active controller type.
	/// </summary>
	public static ControllerType GetActiveControllerType()
	{
		if (!OVRManager.instance.isVRPresent)
			return ControllerType.None;

		return OVRManager.input.activeControllerType;
	}

	/// <summary>
	/// Activates vibration with the given frequency and amplitude on the Active controller.
	/// Ignored if the Active controller does not support vibration. Expected values range from 0 to 1.
	/// </summary>
	public static void SetControllerVibration(float frequency, float amplitude)
	{
		OVRManager.input.SetControllerVibrationInternal(frequency, amplitude, ControllerType.Active);
	}

	/// <summary>
	/// Activates vibration with the given frequency and amplitude with the given controller mask.
	/// Ignored on controllers that do not support vibration. Expected values range from 0 to 1.
	/// </summary>
	public static void SetControllerVibration(float frequency, float amplitude, ControllerType controllerMask)
	{
		OVRManager.input.SetControllerVibrationInternal(frequency, amplitude, controllerMask);
	}

	private void SetControllerVibrationInternal(float frequency, float amplitude, ControllerType controllerMask)
	{
		if (!OVRManager.instance.isVRPresent)
			return;

		if ((controllerMask & ControllerType.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				controller.SetControllerVibration(frequency, amplitude);
			}
		}
	}

	private Vector2 CalculateMax(Vector2 a, Vector2 b)
	{
		float absA = a.sqrMagnitude;
		float absB = b.sqrMagnitude;

		if (absA >= absB)
			return a;
		return b;
	}

	private float CalculateMax(float a, float b)
	{
		float absA = (a >= 0) ? a : -a;
		float absB = (b >= 0) ? b : -b;

		if (absA >= absB)
			return a;
		return b;
	}

	private bool ShouldResolveController(ControllerType controllerType, ControllerType controllerMask)
	{
		bool isValid = false;

		if ((controllerType & controllerMask) == controllerType)
		{
			isValid = true;
		}

		// If the mask requests both Touch controllers, reject the individual touch controllers.
		if (((controllerMask & ControllerType.Touch) == ControllerType.Touch)
			&& ((controllerType & ControllerType.Touch) != 0)
			&& ((controllerType & ControllerType.Touch) != ControllerType.Touch))
		{
			isValid = false;
		}

		return isValid;
	}

	private abstract class OVRControllerBase
	{
		public class VirtualButtonMap
		{
			public RawButton None                     = RawButton.None;
			public RawButton One                      = RawButton.None;
			public RawButton Two                      = RawButton.None;
			public RawButton Three                    = RawButton.None;
			public RawButton Four                     = RawButton.None;
			public RawButton Start                    = RawButton.None;
			public RawButton Back                     = RawButton.None;
			public RawButton PrimaryShoulder          = RawButton.None;
			public RawButton PrimaryIndexTrigger      = RawButton.None;
			public RawButton PrimaryHandTrigger       = RawButton.None;
			public RawButton PrimaryThumbstick        = RawButton.None;
			public RawButton PrimaryThumbstickUp      = RawButton.None;
			public RawButton PrimaryThumbstickDown    = RawButton.None;
			public RawButton PrimaryThumbstickLeft    = RawButton.None;
			public RawButton PrimaryThumbstickRight   = RawButton.None;
			public RawButton SecondaryShoulder        = RawButton.None;
			public RawButton SecondaryIndexTrigger    = RawButton.None;
			public RawButton SecondaryHandTrigger     = RawButton.None;
			public RawButton SecondaryThumbstick      = RawButton.None;
			public RawButton SecondaryThumbstickUp    = RawButton.None;
			public RawButton SecondaryThumbstickDown  = RawButton.None;
			public RawButton SecondaryThumbstickLeft  = RawButton.None;
			public RawButton SecondaryThumbstickRight = RawButton.None;
			public RawButton DpadUp                   = RawButton.None;
			public RawButton DpadDown                 = RawButton.None;
			public RawButton DpadLeft                 = RawButton.None;
			public RawButton DpadRight                = RawButton.None;
			public RawButton Up                       = RawButton.None;
			public RawButton Down                     = RawButton.None;
			public RawButton Left                     = RawButton.None;
			public RawButton Right                    = RawButton.None;

			public RawButton ToRawMask(Button virtualMask)
			{
				RawButton rawMask = 0;

				if (virtualMask == Button.None)
					return RawButton.None;

				if ((virtualMask & Button.One) != 0)
					rawMask |= One;
				if ((virtualMask & Button.Two) != 0)
					rawMask |= Two;
				if ((virtualMask & Button.Three) != 0)
					rawMask |= Three;
				if ((virtualMask & Button.Four) != 0)
					rawMask |= Four;
				if ((virtualMask & Button.Start) != 0)
					rawMask |= Start;
				if ((virtualMask & Button.Back) != 0)
					rawMask |= Back;
				if ((virtualMask & Button.PrimaryShoulder) != 0)
					rawMask |= PrimaryShoulder;
				if ((virtualMask & Button.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Button.PrimaryHandTrigger) != 0)
					rawMask |= PrimaryHandTrigger;
				if ((virtualMask & Button.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Button.PrimaryThumbstickUp) != 0)
					rawMask |= PrimaryThumbstickUp;
				if ((virtualMask & Button.PrimaryThumbstickDown) != 0)
					rawMask |= PrimaryThumbstickDown;
				if ((virtualMask & Button.PrimaryThumbstickLeft) != 0)
					rawMask |= PrimaryThumbstickLeft;
				if ((virtualMask & Button.PrimaryThumbstickRight) != 0)
					rawMask |= PrimaryThumbstickRight;
				if ((virtualMask & Button.SecondaryShoulder) != 0)
					rawMask |= SecondaryShoulder;
				if ((virtualMask & Button.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Button.SecondaryHandTrigger) != 0)
					rawMask |= SecondaryHandTrigger;
				if ((virtualMask & Button.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;
				if ((virtualMask & Button.SecondaryThumbstickUp) != 0)
					rawMask |= SecondaryThumbstickUp;
				if ((virtualMask & Button.SecondaryThumbstickDown) != 0)
					rawMask |= SecondaryThumbstickDown;
				if ((virtualMask & Button.SecondaryThumbstickLeft) != 0)
					rawMask |= SecondaryThumbstickLeft;
				if ((virtualMask & Button.SecondaryThumbstickRight) != 0)
					rawMask |= SecondaryThumbstickRight;
				if ((virtualMask & Button.DpadUp) != 0)
					rawMask |= DpadUp;
				if ((virtualMask & Button.DpadDown) != 0)
					rawMask |= DpadDown;
				if ((virtualMask & Button.DpadLeft) != 0)
					rawMask |= DpadLeft;
				if ((virtualMask & Button.DpadRight) != 0)
					rawMask |= DpadRight;
				if ((virtualMask & Button.Up) != 0)
					rawMask |= Up;
				if ((virtualMask & Button.Down) != 0)
					rawMask |= Down;
				if ((virtualMask & Button.Left) != 0)
					rawMask |= Left;
				if ((virtualMask & Button.Right) != 0)
					rawMask |= Right;

				return rawMask;
			}
		}

		public class VirtualTouchMap
		{
			public RawTouch None                      = RawTouch.None;
			public RawTouch One                       = RawTouch.None;
			public RawTouch Two                       = RawTouch.None;
			public RawTouch Three                     = RawTouch.None;
			public RawTouch Four                      = RawTouch.None;
			public RawTouch PrimaryIndexTrigger       = RawTouch.None;
			public RawTouch PrimaryThumbstick         = RawTouch.None;
			public RawTouch SecondaryIndexTrigger     = RawTouch.None;
			public RawTouch SecondaryThumbstick       = RawTouch.None;

			public RawTouch ToRawMask(Touch virtualMask)
			{
				RawTouch rawMask = 0;

				if (virtualMask == Touch.None)
					return RawTouch.None;

				if ((virtualMask & Touch.One) != 0)
					rawMask |= One;
				if ((virtualMask & Touch.Two) != 0)
					rawMask |= Two;
				if ((virtualMask & Touch.Three) != 0)
					rawMask |= Three;
				if ((virtualMask & Touch.Four) != 0)
					rawMask |= Four;
				if ((virtualMask & Touch.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Touch.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Touch.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Touch.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;

				return rawMask;
			}
		}

		public class VirtualNearTouchMap
		{
			public RawNearTouch None                      = RawNearTouch.None;
			public RawNearTouch PrimaryIndexTrigger       = RawNearTouch.None;
			public RawNearTouch PrimaryThumbButtons       = RawNearTouch.None;
			public RawNearTouch SecondaryIndexTrigger     = RawNearTouch.None;
			public RawNearTouch SecondaryThumbButtons     = RawNearTouch.None;

			public RawNearTouch ToRawMask(NearTouch virtualMask)
			{
				RawNearTouch rawMask = 0;

				if (virtualMask == NearTouch.None)
					return RawNearTouch.None;

				if ((virtualMask & NearTouch.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & NearTouch.PrimaryThumbButtons) != 0)
					rawMask |= PrimaryThumbButtons;
				if ((virtualMask & NearTouch.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & NearTouch.SecondaryThumbButtons) != 0)
					rawMask |= SecondaryThumbButtons;

				return rawMask;
			}
		}

		public class VirtualAxis1DMap
		{
			public RawAxis1D None                      = RawAxis1D.None;
			public RawAxis1D PrimaryIndexTrigger       = RawAxis1D.None;
			public RawAxis1D PrimaryHandTrigger        = RawAxis1D.None;
			public RawAxis1D SecondaryIndexTrigger     = RawAxis1D.None;
			public RawAxis1D SecondaryHandTrigger      = RawAxis1D.None;

			public RawAxis1D ToRawMask(Axis1D virtualMask)
			{
				RawAxis1D rawMask = 0;

				if (virtualMask == Axis1D.None)
					return RawAxis1D.None;

				if ((virtualMask & Axis1D.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Axis1D.PrimaryHandTrigger) != 0)
					rawMask |= PrimaryHandTrigger;
				if ((virtualMask & Axis1D.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Axis1D.SecondaryHandTrigger) != 0)
					rawMask |= SecondaryHandTrigger;

				return rawMask;
			}
		}

		public class VirtualAxis2DMap
		{
			public RawAxis2D None                      = RawAxis2D.None;
			public RawAxis2D PrimaryThumbstick         = RawAxis2D.None;
			public RawAxis2D SecondaryThumbstick       = RawAxis2D.None;

			public RawAxis2D ToRawMask(Axis2D virtualMask)
			{
				RawAxis2D rawMask = 0;

				if (virtualMask == Axis2D.None)
					return RawAxis2D.None;

				if ((virtualMask & Axis2D.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Axis2D.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;

				return rawMask;
			}
		}

		public ControllerType controllerType = ControllerType.None;
		public VirtualButtonMap buttonMap = new VirtualButtonMap();
		public VirtualTouchMap touchMap = new VirtualTouchMap();
		public VirtualNearTouchMap nearTouchMap = new VirtualNearTouchMap();
		public VirtualAxis1DMap axis1DMap = new VirtualAxis1DMap();
		public VirtualAxis2DMap axis2DMap = new VirtualAxis2DMap();
		public OVRPlugin.InputState previousInputState = new OVRPlugin.InputState();
		public OVRPlugin.InputState currentInputState = new OVRPlugin.InputState();

		public OVRControllerBase()
		{
			ConfigureButtonMap();
			ConfigureTouchMap();
			ConfigureNearTouchMap();
			ConfigureAxis1DMap();
			ConfigureAxis2DMap();
		}

		public virtual ControllerType Update()
		{
			OVRPlugin.InputState state = OVRPlugin.GetInputState((uint)controllerType);

			if (state.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LIndexTrigger;
			if (state.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LHandTrigger;
			if (state.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickUp;
			if (state.LThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickDown;
			if (state.LThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickLeft;
			if (state.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickRight;

			if (state.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RIndexTrigger;
			if (state.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RHandTrigger;
			if (state.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickUp;
			if (state.RThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickDown;
			if (state.RThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickLeft;
			if (state.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickRight;

			previousInputState = currentInputState;
			currentInputState = state;

			return ((ControllerType)currentInputState.ConnectedControllerTypes & controllerType);
		}

		public virtual void SetControllerVibration(float frequency, float amplitude)
		{
			OVRPlugin.SetControllerVibration((uint)controllerType, frequency, amplitude);
		}

		public abstract void ConfigureButtonMap();
		public abstract void ConfigureTouchMap();
		public abstract void ConfigureNearTouchMap();
		public abstract void ConfigureAxis1DMap();
		public abstract void ConfigureAxis2DMap();

		public RawButton ResolveToRawMask(Button virtualMask)
		{
			return buttonMap.ToRawMask(virtualMask);
		}

		public RawTouch ResolveToRawMask(Touch virtualMask)
		{
			return touchMap.ToRawMask(virtualMask);
		}

		public RawNearTouch ResolveToRawMask(NearTouch virtualMask)
		{
			return nearTouchMap.ToRawMask(virtualMask);
		}

		public RawAxis1D ResolveToRawMask(Axis1D virtualMask)
		{
			return axis1DMap.ToRawMask(virtualMask);
		}

		public RawAxis2D ResolveToRawMask(Axis2D virtualMask)
		{
			return axis2DMap.ToRawMask(virtualMask);
		}
	}

	private class OVRControllerTouch : OVRControllerBase
	{
		public OVRControllerTouch()
		{
			controllerType = ControllerType.Touch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.None;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.RHandTrigger;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.A;
			touchMap.Two                       = RawTouch.B;
			touchMap.Three                     = RawTouch.X;
			touchMap.Four                      = RawTouch.Y;
			touchMap.PrimaryIndexTrigger       = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.LThumbstick;
			touchMap.SecondaryIndexTrigger     = RawTouch.RIndexTrigger;
			touchMap.SecondaryThumbstick       = RawTouch.RThumbstick;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.RIndexTrigger;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.RThumbButtons;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.RHandTrigger;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
		}
	}

	private class OVRControllerLTouch : OVRControllerBase
	{
		public OVRControllerLTouch()
		{
			controllerType = ControllerType.LTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.X;
			buttonMap.Two                      = RawButton.Y;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.None;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.X;
			touchMap.Two                       = RawTouch.Y;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.LThumbstick;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
		}
	}

	private class OVRControllerRTouch : OVRControllerBase
	{
		public OVRControllerRTouch()
		{
			controllerType = ControllerType.RTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.None;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.RIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.RHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.RThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.RThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.RThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.RThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.RThumbstickRight;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.RThumbstickUp;
			buttonMap.Down                     = RawButton.RThumbstickDown;
			buttonMap.Left                     = RawButton.RThumbstickLeft;
			buttonMap.Right                    = RawButton.RThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.A;
			touchMap.Two                       = RawTouch.B;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.RIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.RThumbstick;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.RIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.RThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.RIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.RHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
		}
	}

	private class OVRControllerXbox : OVRControllerBase
	{
		/// <summary> An axis on the gamepad. </summary>
		private enum AxisGPC
		{
			None = -1,
			LeftXAxis = 0,
			LeftYAxis,
			RightXAxis,
			RightYAxis,
			LeftTrigger,
			RightTrigger,
			DPad_X_Axis,
			DPad_Y_Axis,
			Max,
		};

		/// <summary> A button on the gamepad. </summary>
		public enum ButtonGPC
		{
			None = -1,
			A = 0,
			B,
			X,
			Y,
			Up,
			Down,
			Left,
			Right,
			Start,
			Back,
			LStick,
			RStick,
			LeftShoulder,
			RightShoulder,
			Max
		};

		private bool initialized = false;
		private bool joystickDetected = false;
		private float joystickCheckInterval = 1.0f;
		private float joystickCheckTime = 0.0f;

		public OVRControllerXbox()
		{
			controllerType = ControllerType.Xbox;

			initialized = OVR_GamepadController_Initialize();
		}

		~OVRControllerXbox()
		{
			if (!initialized)
				return;

			OVR_GamepadController_Destroy();
		}

		private bool ShouldUpdate()
		{
			// XInput is notoriously slow to update if no Xbox controllers are present. (up to ~0.5 ms)
			// Use Unity's joystick detection as a quick way to short-circuit the need to query XInput.
			if ((Time.time - joystickCheckTime) > joystickCheckInterval)
			{
				joystickCheckTime = Time.time;
				joystickDetected = false;
				var joystickNames = UnityEngine.Input.GetJoystickNames();

				for (int i = 0; i < joystickNames.Length; i++)
				{
					if (joystickNames[i] != String.Empty)
					{
						joystickDetected = true;
						break;
					}
				}
			}

			return joystickDetected;
		}

		public override ControllerType Update()
		{
			if (!initialized || !ShouldUpdate())
			{
				return ControllerType.None;
			}

			OVRPlugin.InputState state = new OVRPlugin.InputState();

			bool result = OVR_GamepadController_Update();

			if (result)
				state.ConnectedControllerTypes = (uint)ControllerType.Xbox;

			if (OVR_GamepadController_GetButton((int)ButtonGPC.A))
				state.Buttons |= (uint)RawButton.A;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.B))
				state.Buttons |= (uint)RawButton.B;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.X))
				state.Buttons |= (uint)RawButton.X;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Y))
				state.Buttons |= (uint)RawButton.Y;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Up))
				state.Buttons |= (uint)RawButton.DpadUp;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Down))
				state.Buttons |= (uint)RawButton.DpadDown;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Left))
				state.Buttons |= (uint)RawButton.DpadLeft;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Right))
				state.Buttons |= (uint)RawButton.DpadRight;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Start))
				state.Buttons |= (uint)RawButton.Start;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Back))
				state.Buttons |= (uint)RawButton.Back;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.LStick))
				state.Buttons |= (uint)RawButton.LThumbstick;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.RStick))
				state.Buttons |= (uint)RawButton.RThumbstick;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.LeftShoulder))
				state.Buttons |= (uint)RawButton.LShoulder;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.RightShoulder))
				state.Buttons |= (uint)RawButton.RShoulder;

			state.LThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.LeftXAxis);
			state.LThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.LeftYAxis);
			state.RThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.RightXAxis);
			state.RThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.RightYAxis);
			state.LIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.LeftTrigger);
			state.RIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.RightTrigger);

			if (state.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LIndexTrigger;
			if (state.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LHandTrigger;
			if (state.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickUp;
			if (state.LThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickDown;
			if (state.LThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickLeft;
			if (state.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickRight;

			if (state.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RIndexTrigger;
			if (state.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RHandTrigger;
			if (state.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickUp;
			if (state.RThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickDown;
			if (state.RThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickLeft;
			if (state.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickRight;

			previousInputState = currentInputState;
			currentInputState = state;

			return ((ControllerType)currentInputState.ConnectedControllerTypes & controllerType);
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.Back;
			buttonMap.PrimaryShoulder          = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.SecondaryShoulder        = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.DpadUp                   = RawButton.DpadUp;
			buttonMap.DpadDown                 = RawButton.DpadDown;
			buttonMap.DpadLeft                 = RawButton.DpadLeft;
			buttonMap.DpadRight                = RawButton.DpadRight;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
		}

		public override void SetControllerVibration(float frequency, float amplitude)
		{
			int gpcNode = 0;
			float gpcFrequency = frequency * 200.0f; //Map frequency from 0-1 CAPI range to 0-200 GPC range
			float gpcStrength = amplitude;

			OVR_GamepadController_SetVibration(gpcNode, gpcStrength, gpcFrequency);
		}

		private const string DllName = "OVRGamepad";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Initialize();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Destroy();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Update();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern float OVR_GamepadController_GetAxis(int axis);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_GetButton(int button);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_SetVibration(int node, float strength, float frequency);
	}
}
