using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class AndroidPermissionHandler : MonoBehaviour
{
	bool isItPermissionTime = false;
	string nextPermission;
	Stack<string> permissions = new Stack<string>();

	void Start()
    {
		OpenAllPermissions();
	}

	public void OpenAllPermissions()
	{
		isItPermissionTime = true;
		CreatePermissionList();
	
	}

	void CreatePermissionList()
	{
		permissions = new Stack<string>();
		permissions.Push(Permission.ExternalStorageWrite);
		permissions.Push(Permission.Camera);
		permissions.Push(Permission.CoarseLocation);
		AskForPermissions();
	}

     void AskForPermissions ()
	{
		if (permissions == null || permissions.Count <= 0)
		{
			isItPermissionTime = false;
			return;
		}
		nextPermission = permissions.Pop();

		if (nextPermission == null)
		{
			isItPermissionTime = false;
			return;
		}
		if (Permission.HasUserAuthorizedPermission(nextPermission) == false)
		{
			Permission.RequestUserPermission(nextPermission);
		}
		else
		{
			if (isItPermissionTime == true)
				AskForPermissions();
		}


		Debug.Log("Unity>> permission " + nextPermission + "  status ;" + Permission.HasUserAuthorizedPermission(nextPermission));

	}

	private void OnApplicationFocus(bool focus)
	{
		Debug.Log("Unity>> focus ....  " + focus + "   isPermissionTime : " + isItPermissionTime);
		if (focus == true && isItPermissionTime == true)
		{
			AskForPermissions();
		}
	}
}
