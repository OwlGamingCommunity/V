using System;

public class Vector2
{
	public Vector2(float a_fX, float a_fY)
	{
		X = a_fX;
		Y = a_fY;
		SetSuccessfully = true;
	}

	public Vector2(float a_fX, float a_fY, bool a_bSetSuccessfully)
	{
		X = a_fX;
		Y = a_fY;
		SetSuccessfully = a_bSetSuccessfully;
	}

	public float X { get; set; }
	public float Y { get; set; }
	public bool SetSuccessfully { get; set; }
}

public class Vector4
{
	public Vector4(float a_fX, float a_fY, float a_fZ, float a_fW)
	{
		X = a_fX;
		Y = a_fY;
		Z = a_fZ;
		W = a_fW;
		SetSuccessfully = true;
	}

	public Vector4(float a_fX, float a_fY, float a_fZ, float a_fW, bool a_bSetSuccessfully)
	{
		X = a_fX;
		Y = a_fY;
		Z = a_fZ;
		W = a_fW;
		SetSuccessfully = a_bSetSuccessfully;
	}

	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public float W { get; set; }
	public bool SetSuccessfully { get; set; }
}

public static class GraphicsHelper
{
	public static Vector2 GetScreenResolution()
	{
		int resX = 0;
		int resY = 0;
		RAGE.Game.Graphics.GetActiveScreenResolution(ref resX, ref resY);
		return new Vector2(resX, resY);
	}

	public static Vector2 GetScreenPositionFromWorldPosition(RAGE.Vector3 a_vecPos)
	{
		float fScreenX = 0.0f;
		float fScreenY = 0.0f;

		bool bSuccess = RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(a_vecPos.X, a_vecPos.Y, a_vecPos.Z, ref fScreenX, ref fScreenY);

		return new Vector2(fScreenX, fScreenY, bSuccess);
	}

	public static RAGE.Vector3 GetWorldPositionFromScreenPosition(Vector2 a_vecPos)
	{
		RAGE.Vector3 vecCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);
		Vector2 vecProcessedCoords = InternalMathHelpers.ProcessCoordinates(a_vecPos);

		RAGE.Vector3 target = InternalMathHelpers.Screen2World(vecCamPos, vecProcessedCoords.X, vecProcessedCoords.Y);
		RAGE.Vector3 direction = target - vecCamPos;
		RAGE.Vector3 from = vecCamPos;
		RAGE.Vector3 to = vecCamPos + direction.Multiply(300.0f);

		CRaycastResult ray = WorldHelper.RaycastFromTo(from, to, -1, 1);
		return ray.Hit ? ray.EndCoords : null;
	}

	private static class InternalMathHelpers
	{
		public static Vector2 ProcessCoordinates(Vector2 vecPos)
		{
			Vector2 vecResolution = GraphicsHelper.GetScreenResolution();

			float relativeX = (1.0f - ((vecPos.X / vecResolution.X) * 1.0f) * 2.0f);
			float relativeY = (1.0f - ((vecPos.Y / vecResolution.Y) * 1.0f) * 2.0f);

			if (relativeX > 0.0f)
			{
				relativeX = -relativeX;
			}
			else
			{
				relativeX = Math.Abs(relativeX);
			}

			if (relativeY > 0.0f)
			{
				relativeY = -relativeY;
			}
			else
			{
				relativeY = Math.Abs(relativeY);
			}

			return new Vector2(relativeX, relativeY);
		}
		public static Vector2 World2Screen(RAGE.Vector3 vecWorld)
		{
			float fScreenX = 0.0f;
			float fScreenY = 0.0f;
			bool bSuccess = RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(vecWorld.X, vecWorld.Y, vecWorld.Z, ref fScreenX, ref fScreenY);

			return new Vector2((float)(fScreenX - 0.5) * 2.0f, (float)(fScreenY - 0.5) * 2.0f, bSuccess);
		}

		public static RAGE.Vector3 Screen2World(RAGE.Vector3 vecCamPos, float relX, float relY)
		{
			RAGE.Vector3 camRot = CameraManager.GetCameraRotation(ECameraID.GAME);
			RAGE.Vector3 camForward = MathHelpers.RotationToDirection(camRot);
			RAGE.Vector3 rotUp = camRot + new RAGE.Vector3(10.0f, 0.0f, 0.0f);
			RAGE.Vector3 rotDown = camRot + new RAGE.Vector3(-10.0f, 0.0f, 0.0f);
			RAGE.Vector3 rotLeft = camRot + new RAGE.Vector3(0.0f, 0.0f, -10.0f);
			RAGE.Vector3 rotRight = camRot + new RAGE.Vector3(0.0f, 0.0f, 10.0f);

			RAGE.Vector3 camRight = MathHelpers.RotationToDirection(rotRight) - MathHelpers.RotationToDirection(rotLeft);
			RAGE.Vector3 camUp = MathHelpers.RotationToDirection(rotUp) - MathHelpers.RotationToDirection(rotDown);

			float rollRad = -MathHelpers.DegreesToRadians(camRot.Y);

			RAGE.Vector3 camRightRoll = camRight.Multiply((float)Math.Cos(rollRad)) - camUp.Multiply((float)Math.Sin(rollRad));
			RAGE.Vector3 camUpRoll = camRight.Multiply((float)Math.Sin(rollRad)) + camUp.Multiply((float)Math.Cos(rollRad));

			RAGE.Vector3 point3D = (vecCamPos + camForward.Multiply(10.0f) + camRightRoll) + camUpRoll;

			Vector2 point2D = World2Screen(point3D);

			if (!point2D.SetSuccessfully)
			{
				return vecCamPos + camForward.Multiply(10.0f);
			}

			RAGE.Vector3 point3DZero = vecCamPos + camForward.Multiply(10.0f);
			Vector2 point2DZero = World2Screen(point3DZero);

			if (!point2D.SetSuccessfully)
			{
				return vecCamPos + camForward.Multiply(10.0f);
			}

			float eps = 0.001f;

			if (Math.Abs(point2D.X - point2DZero.X) < eps || Math.Abs(point2D.Y - point2DZero.Y) < eps)
			{
				return vecCamPos + camForward.Multiply(10.0f);
			}

			float scaleX = (relX - point2DZero.X) / (point2D.X - point2DZero.X);
			float scaleY = (relY - point2DZero.Y) / (point2D.Y - point2DZero.Y);

			RAGE.Vector3 point3Dret = ((vecCamPos + camForward.Multiply(10.0f) + camRightRoll.Multiply(scaleX)) + camUpRoll.Multiply(scaleY));
			return point3Dret;
		}
	}

}

public static class MathHelpers
{
	public static float DegreesToRadians(float fDegrees)
	{
		return (float)(fDegrees * Math.PI / 180.0f);
	}

	public static RAGE.Vector3 RotationToDirection(RAGE.Vector3 vecRot)
	{
		float z = DegreesToRadians(vecRot.Z);
		float x = DegreesToRadians(vecRot.X);
		float fNum = (float)Math.Abs(Math.Cos(x));

		return new RAGE.Vector3((float)(-Math.Sin(z) * fNum), (float)(Math.Cos(z) * fNum), (float)Math.Sin(x));
	}

	public static RAGE.Vector3 OffsetPosition(RAGE.Vector3 vecPos, float heading, float distance)
	{
		RAGE.Vector3 retVal = new RAGE.Vector3();

		retVal.X = vecPos.X + (float)Math.Sin(-heading * Math.PI / 180) * distance;
		retVal.Y = vecPos.Y + (float)Math.Cos(-heading * Math.PI / 180) * distance;
		retVal.Z = vecPos.Z;

		return retVal;
	}
}