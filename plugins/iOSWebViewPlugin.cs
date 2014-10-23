using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Plugins
{
	public class iOSWebViewPlugin : IWebViewPlugin
	{
		private IntPtr _webView;

		[DllImport("__Internal")]
		private static extern IntPtr _WebViewPlugin_Init(string gameObject);
		[DllImport("__Internal")]
		private static extern int _WebViewPlugin_Destroy(IntPtr instance);
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_SetMargins(
			IntPtr instance, int left, int top, int right, int bottom);
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_SetVisibility(
			IntPtr instance, bool visibility);
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_LoadURL(
			IntPtr instance, string url);
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_EvaluateJS(
			IntPtr instance, string url);
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_SetFrame(
			IntPtr instance, int x, int y, int width, int height);

		public void Init(string name)
		{
			_webView = _WebViewPlugin_Init(name);
		}

		public bool KeyboardVisible
		{
			get
			{
#if UNITY_IPHONE
				return TouchScreenKeyboard.visible;
#else
				return false;
#endif
			}
		}

		public void OnDestroy()
		{
			if (_webView == IntPtr.Zero)
				return;
			_WebViewPlugin_Destroy(_webView);
		}

		public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
		{
			if (_webView == IntPtr.Zero) 
				return;
			_WebViewPlugin_SetFrame(_webView, (int)center.x, (int)center.y, (int)scale.x, (int)scale.y);
		}

		public void SetMargins(int left, int top, int right, int bottom)
		{
			if (_webView == IntPtr.Zero)
				return;
			_WebViewPlugin_SetMargins(_webView, left, top, right, bottom);
		}

		public void SetVisibility(bool v)
		{
			if (_webView == IntPtr.Zero)
				return;
			_WebViewPlugin_SetVisibility(_webView, v);
		}

		public void LoadUrl(string url)
		{
			if (_webView == IntPtr.Zero)
				return;
			_WebViewPlugin_LoadURL(_webView, GetAbsoluteUrl(url));
		}

		public void EvaluateJs(string js)
		{
			if (_webView == IntPtr.Zero)
				return;
			_WebViewPlugin_EvaluateJS(_webView, js);
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
			//not used on iOS
		}

		public void OnGui()
		{
			//not used on iOS
		}
	}
}
