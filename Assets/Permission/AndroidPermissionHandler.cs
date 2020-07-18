using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class AndroidPermissionHandler : MonoBehaviour
{
	public static AndroidPermissionHandler instance;

	private bool _isPermissionRequestAskingProcessOnGoing = false;
	private Stack<string> _permissions = new Stack<string>();
	private Delegate _delegate;
	private int _identifier;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	#region PublicApi
	public void RequestSinglePermission(string permission,Delegate completationDelegate,int identifier)
	{
		Stack<string> permissions = new Stack<string>();
		permissions.Push(permission);
		RequestMultiplePermission(permissions, completationDelegate,identifier);
	}
	public void RequestMultiplePermission(Stack<string> permissions, Delegate completationDelegate,int identifier)
	{
		this._delegate = completationDelegate;
		this._identifier = identifier;
		ClearPermissionStack();
		this._permissions = permissions;
		_isPermissionRequestAskingProcessOnGoing = true;

		ShowPermissionDialog();
	}
	public bool HasUserPermission(string permission)
	{
		return Permission.HasUserAuthorizedPermission(permission);
	}
	#endregion PublicApi


	#region PrivateApi
	private void ShowPermissionDialog ()
	{
		if (_permissions == null || _permissions.Count <= 0)
		{
			CompleteAskingPermission();
			return;
		}
		string permission = _permissions.Pop();
		if (HasUserPermission(permission) == false)
		{
			Permission.RequestUserPermission(permission);
		}
		else
		{
			if (_isPermissionRequestAskingProcessOnGoing == true)
				ShowPermissionDialog();
		}
		Debug.Log("Unity>> permission " + permission + "  status ;" + Permission.HasUserAuthorizedPermission(permission));
	}
	private void CompleteAskingPermission()
	{
		_isPermissionRequestAskingProcessOnGoing = false;
		if (_delegate != null)
			_delegate.OnCompleteAskingPermissionRequest(_identifier);
	}
	private void OnApplicationFocus(bool focus)
	{
		Debug.Log("Unity>> focus ....  " + focus + "   isPermissionTime : " + _isPermissionRequestAskingProcessOnGoing);
		if (focus == true && _isPermissionRequestAskingProcessOnGoing == true)
		{
			ShowPermissionDialog();
		}
	}
	private void ClearPermissionStack()
	{
		if(_permissions == null)
			_permissions = new Stack<string>();

		_permissions.Clear();
	}

# endregion PrivateApi

	public interface Delegate
	{
		void OnCompleteAskingPermissionRequest( int identifier);
	}
}
