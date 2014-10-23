using UnityEngine;

#if UNITY_ANDROID
namespace Assets.Plugins
{
	public class AndroidWebViewPlugin : IWebViewPlugin
	{
		private AndroidJavaObject _webView;

		public void Init(string name)
		{
			_webView = new AndroidJavaObject("net.gree.unitywebview.WebViewPlugin");
			_webView.Call("Init", name);
		}

		public AndroidWebViewPlugin()
		{
			KeyboardVisible = false;
		}

		public bool KeyboardVisible { get; set; }

		public void OnDestroy()
		{
			if (_webView == null)
				return;
			_webView.Call("Destroy");
		}

		public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
		{
			//not implemented for android?
		}

		public void SetMargins(int left, int top, int right, int bottom)
		{
			if (_webView == null)
				return;
			_webView.Call("SetMargins", left, top, right, bottom);
		}

		public void SetVisibility(bool v)
		{
			if (_webView == null)
				return;
			_webView.Call("SetVisibility", v);
		}

		public void LoadUrl(string url)
		{
			if (_webView == null)
				return;
			_webView.Call("LoadURL", GetAbsoluteUrl(url));
		}

		public void EvaluateJs(string js)
		{
			if (_webView == null)
				return;
			_webView.Call("LoadURL", "javascript:" + js);
		}

		public string GetAbsoluteUrl(string url)
		{
			if (url.StartsWith("http://"))
			{
				return url;
			}
			return "file:///android_asset/" + url;
		}

		public void AddNecessaryJavascriptEvents()
		{
			//nothing for android
		}

		public void OnUpdate()
		{
			//nothing for android
		}

		public void OnGui()
		{
			//nothing for android
		}
	}
}
#endif
