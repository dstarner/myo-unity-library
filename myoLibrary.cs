using UnityEngine;
using System.Collections;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

/**
 * Class to make myo movement hopefully easier
 * TO USE:
 * 1. Create instance of MyoMovement with "MyoMovement myoMovement = new MyoMovement(myo);
 * 2. In Update() of the object, call myoMovement.Update();
 * 3. Call by using myMovement.*desiredFunction*()
 * 
 **/
public class MyoMovement {
	private GameObject myo;
	private ThalmicMyo thalmicMyo;
	private Pose _lastPose = Pose.Unknown;
	private Pose curPose;
	public bool isMyo = false;
	//For position
	public float curX = 0f;
	public float curY = 0f;
	private float curZ = 0f;
	public float xOffset = 0f;
	public float yOffset = 0f;
	private float zOffset = 0f;

	//Has been calculated
	private bool hasIn = false;
	private bool hasOut = false;
	private bool hasDouble = false;
	private bool hasFist = false;
	private bool hasSpread = false;

	//Used solely on startup
	private bool isFirstUpdate = true;

	// Use this for initialization
	public MyoMovement (GameObject myo) {
		this.myo = myo;
		this.thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		if (thalmicMyo.isPaired) {
			isMyo = true;
		}
		curPose = _lastPose;
	}

	/// <summary>
	/// Updates the current pose.
	/// </summary>
	public void Update() {
		if (isFirstUpdate) { //Called to fix offsets
			xOffset = myo.transform.rotation.x;
			yOffset = myo.transform.rotation.y;
			zOffset = myo.transform.rotation.z;
			isFirstUpdate = false;
		} //End isFirstUpdate check

		//Update current values
		curX = myo.transform.rotation.x;
		curY = myo.transform.rotation.y;
		curZ = myo.transform.rotation.z;
		//End value update

		if (curPose != thalmicMyo.pose) { //Updates pose
			falsifyOnce();
			_lastPose = curPose;
			curPose = thalmicMyo.pose;
		} //End update pose
	}

	/// <summary>
	/// Gets the raw angles as a Vector3, with no influence from Offsets.
	/// </summary>
	/// <returns>The raw angles.</returns>
	public Vector3 getRawAngles() {
		return new Vector3 (curX, curY, curZ);
	}

	/// <summary>
	/// Gets the offset values as Vector3.
	/// </summary>
	/// <returns>The offset values.</returns>
	public Vector3 getOffsetValues() {
		return new Vector3 (xOffset, yOffset, zOffset);
	}

	/// <summary>
	/// Gets the current local angles using the offsets as Vector3 (desired).
	/// </summary>
	/// <returns>The angles.</returns>
	public Vector3 getAngles() {
		return new Vector3 (normalizeAngle(curX - xOffset), normalizeAngle(curY - yOffset), normalizeAngle(curZ - zOffset));
	}

	/// <summary>
	/// Gets the current local angles using the offsets as Vector3 (desired) with sensitivity.
	/// </summary>
	/// <returns>The angles.</returns>
	public Vector3 getSensitiveAngles(Vector3 senses) {
		return new Vector3 (normalizeAngle(curX - xOffset) * senses.x, normalizeAngle(curY - yOffset) * senses.y, normalizeAngle(curZ - zOffset) * senses.z);
	}

	/// <summary>
	/// Gets the current local angles using the offsets as Vector3 (desired) with sensitivity.
	/// </summary>
	/// <returns>The angles.</returns>
	public Vector3 getSensitiveAngles(float xSens, float ySens, float zSens) {
		return new Vector3 (normalizeAngle(curX - xOffset) * xSens, normalizeAngle(curY - yOffset) * ySens, normalizeAngle(curZ - zOffset) * zSens);
	}

	/// <summary>
	/// Resets the offsets to current position.
	/// </summary>
	public void resetOffsets() {
		xOffset = myo.transform.rotation.x;
		yOffset = myo.transform.rotation.y;
		zOffset = myo.transform.rotation.z;
	}

	/// <summary>
	/// Resets the offsets to defined position.
	/// </summary>
	public void resetOffsets(Vector3 offsets) {
		xOffset = offsets.x;
		yOffset = offsets.y;
		zOffset = offsets.z;
	}



	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	//********POSES**********************
	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	/// <summary>
	/// Returns true if pose is Fingers Spread.
	/// </summary>
	public bool isFingerSpread() {
		if (curPose == Pose.FingersSpread) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is Unknown.
	/// </summary>
	public bool isUnknown() {
		if (curPose == Pose.Unknown) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is Rest.
	/// </summary>
	public bool isRest() {
		if (curPose == Pose.Rest) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is DoubleTap.
	/// </summary>
	public bool isDoubleTap() {
		if (curPose == Pose.DoubleTap) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is Wave In.
	/// </summary>
	public bool isWaveIn() {
		if (curPose == Pose.WaveIn) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is Wave Out.
	/// </summary>
	public bool isWaveOut() {
		if (curPose == Pose.WaveOut) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if pose is a fist.
	/// </summary>
	public bool isFist() {
		if (curPose == Pose.Fist) {
			return true;
		} else {
			return false;
		}
	}
	/// <summary>
	/// Returns true if pose is Fingers Spread ONCE.
	/// </summary>
	public bool oneFingerSpread() {
		if (curPose == Pose.FingersSpread && !hasSpread) {
			hasSpread = true;
			return true;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// Returns true if pose is DoubleTap ONCE.
	/// </summary>
	public bool oneDoubleTap() {
		if (curPose == Pose.DoubleTap && !hasDouble) {
			hasDouble = true;
			return true;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// Returns true if pose is Wave In.
	/// </summary>
	public bool oneWaveIn() {
		if (curPose == Pose.WaveIn && !hasIn) {
			hasIn = true;
			return true;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// Returns true if pose is Wave Out.
	/// </summary>
	public bool oneWaveOut() {
		if (curPose == Pose.WaveOut && !hasOut) {
			hasOut = true;
			return true;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// Returns true if pose is a fist.
	/// </summary>
	public bool oneFist() {
		if (curPose == Pose.Fist && !hasFist) {
			hasFist = true;
			return true;
		} else {
			return false;
		}
	}


	/// <summary>
	/// Gets the current pose.
	/// </summary>
	public Pose getCurrentPose() {
		curPose = thalmicMyo.pose;
		return curPose;
	}

	public bool myoConnected() {
		return isMyo;
	}

	private void falsifyOnce() {
		hasIn = false;
		hasOut = false;
		hasSpread = false;
		hasFist = false;
		hasDouble = false;
	}

	float normalizeAngle (float angle)
	{
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}
}
