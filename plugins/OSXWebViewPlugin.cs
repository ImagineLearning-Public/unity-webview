using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Plugins
{
	public class OSXWebViewPlugin : IWebViewPlugin
	{
		IntPtr webView;
		bool visibility;
		Rect rect;
		Texture2D texture;
		string inputString;

		[DllImport("WebView")]
		private static extern IntPtr _WebViewPlugin_Init(
			string gameObject, int width, int height, bool ineditor);
		[DllImport("WebView")]
		private static extern int _WebViewPlugin_Destroy(IntPtr instance);
		[DllImport("WebView")]
		private static extern void _WebViewPlugin_SetRect(
			IntPtr instance, int width, int height);
		[DllImport("WebView")]
		private static extern void _WebViewPlugin_SetVisibility(
			IntPtr instance, bool visibility);
		[DllImport("WebView")]
		private static extern void _WebViewPlugin_LoadURL(
			IntPtr instance, string url);
		[DllImport("WebView")]
		private static extern void _WebViewPlugin_EvaluateJS(
			IntPtr instance, string url);
		[DllImport("WebView")]
		private static extern void _WebViewPlugin_Update(IntPtr instance,
			int x, int y, float deltaY, bool down, bool press, bool release,
			bool keyPress, short keyCode, string keyChars, int textureId);

		public void Init(string name)
		{
			CreateTexture(0, 0, Screen.width, Screen.height);
			webView = _WebViewPlugin_Init(name, Screen.width, Screen.height,
				Application.platform == RuntimePlatform.OSXEditor);
		}

		private void CreateTexture(int x, int y, int width, int height)
		{
			int w = 1;
			int h = 1;
			while (w < width)
				w <<= 1;
			while (h < height)
				h <<= 1;
			rect = new Rect(x, y, width, height);
			texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
		}

		public bool KeyboardVisible { get { return false; } }
		public void OnDestroy()
		{
			if (webView == IntPtr.Zero)
				return;
			_WebViewPlugin_Destroy(webView);
		}

		public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
		{
			rect.x = center.x + (Screen.width - scale.x) / 2;
			rect.y = center.y + (Screen.height - scale.y) / 2;
			rect.width = scale.x;
			rect.height = scale.y;
		}

		public void SetMargins(int left, int top, int right, int bottom)
		{
			if (webView == IntPtr.Zero)
				return;
			int width = Screen.width - (left + right);
			int height = Screen.height - (bottom + top);
			CreateTexture(left, bottom, width, height);
			_WebViewPlugin_SetRect(webView, width, height);
		}

		public void SetVisibility(bool v)
		{
			if (webView == IntPtr.Zero)
				return;
			visibility = v;
			_WebViewPlugin_SetVisibility(webView, v);
		}

		public void LoadUrl(string url)
		{
			if (webView == IntPtr.Zero)
				return;
			_WebViewPlugin_LoadURL(webView, GetAbsoluteUrl(url));
		}

		public void EvaluateJs(string js)
		{
			if (webView == IntPtr.Zero)
				return;
			_WebViewPlugin_EvaluateJS(webView, js);
		}

		public string GetAbsoluteUrl(string url)
		{
			if (url.StartsWith("http://"))
			{
				return url;
			}
			return Application.streamingAssetsPath + "/" + url;
		}

		public void AddNecessaryJavascriptEvents()
		{
			EvaluateJs(
					"window.addEventListener('load', function() {" +
					"	window.Unity = {" +
					"		call:function(msg) {" +
					"			var iframe = document.createElement('IFRAME');" +
					"			iframe.setAttribute('src', 'unity:' + msg);" +
					"			document.documentElement.appendChild(iframe);" +
					"			iframe.parentNode.removeChild(iframe);" +
					"			iframe = null;" +
					"		}" +
					"	}" +
					"}, false);");
			EvaluateJs(
				"window.addEventListener('load', function() {" +
				"	window.addEventListener('click', function() {" +
				"		Unity.call('clicked');" +
				"	}, false);" +
				"}, false);");
		}

		public void OnUpdate()
		{
			inputString += Input.inputString;
		}

		public void OnGui()
		{
			if (webView == IntPtr.Zero || !visibility)
				return;

			Vector3 pos = Input.mousePosition;
			bool down = Input.GetButton("Fire1");
			bool press = Input.GetButtonDown("Fire1");
			bool release = Input.GetButtonUp("Fire1");
			float deltaY = Input.GetAxis("Mouse ScrollWheel");
			bool keyPress = false;
			string keyChars = "";
			short keyCode = 0;
			if (inputString.Length > 0)
			{
				keyPress = true;
				keyChars = inputString.Substring(0, 1);
				keyCode = (short)inputString[0];
				inputString = inputString.Substring(1);
			}
			_WebViewPlugin_Update(webView,
				(int)(pos.x - rect.x), (int)(pos.y - rect.y), deltaY,
				down, press, release, keyPress, keyCode, keyChars,
				texture.GetNativeTextureID());
			GL.IssuePluginEvent((int)webView);
			Matrix4x4 m = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, Screen.height, 0),
				Quaternion.identity, new Vector3(1, -1, 1));
			GUI.DrawTexture(rect, texture);
			GUI.matrix = m;
		}
	}
}
