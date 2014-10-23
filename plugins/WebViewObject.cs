/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using Assets.Plugins;
using UnityEngine;
using Callback = System.Action<string>;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
public class UnitySendMessageDispatcher
{
	public static void Dispatch(string name, string method, string message)
	{
		GameObject obj = GameObject.Find(name);
		if (obj != null)
			obj.SendMessage(method, message);
	}
}
#endif

public class WebViewObject : MonoBehaviour
{
	Callback callback;
	private IWebViewPlugin _webViewPlugin;
	
	public void SetKeyboardVisible(string pIsVisible)
	{
		var mIsKeyboardVisible = (pIsVisible == "true");
		//not sure if this is necessary
	}

	public bool IsKeyboardVisible 
	{
		get { return _webViewPlugin.KeyboardVisible; }
	}


	public void Init(Callback cb = null)
	{
		callback = cb;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		_webViewPlugin = new OSXWebViewPlugin();
#elif UNITY_IPHONE
		_webViewPlugin = new iOSWebViewPlugin();
#elif UNITY_ANDROID
		_webViewPlugin = new AndroidWebViewPlugin();
#elif UNITY_WEBPLAYER
		_webViewPlugin = new WebPlayerWebViewPlugin();
#endif
		_webViewPlugin.Init(name);
	}

	void OnDestroy()
	{
		_webViewPlugin.OnDestroy();
	}

    /** Use this function instead of SetMargins to easily set up a centered window */
    public void SetCenterPositionWithScale(Vector2 center , Vector2 scale)
    {
	    _webViewPlugin.SetCenterPositionWithScale(center, scale);
    }

	public void SetMargins(int left, int top, int right, int bottom)
	{
		_webViewPlugin.SetMargins(left, top, right, bottom);
	}

	public void SetVisibility(bool v)
	{
		_webViewPlugin.SetVisibility(v);
	}

	public void LoadURL(string url)
	{
		_webViewPlugin.LoadUrl(url);
	}

	public void EvaluateJS(string js)
	{
		_webViewPlugin.EvaluateJs(js);
	}

	public void CallFromJS(string message)
	{
		if (callback != null)
			callback(message);
	}

	public void AddNecessaryJavascriptEvents()
	{
		_webViewPlugin.AddNecessaryJavascriptEvents();
	}
	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	void Update()
	{
		_webViewPlugin.OnUpdate();
	}

	void OnGUI()
	{
		_webViewPlugin.OnGui();
	}
#endif
}
