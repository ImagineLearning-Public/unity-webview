using UnityEngine;

namespace Assets.Plugins
{
	public class WebPlayerWebViewPlugin : IWebViewPlugin
	{
		private string _name;

		public void Init(string name)
		{
			//this call requires javascript on the page the web player is embedded in
			_name = name;
			Application.ExternalCall("unityWebView.init", name);
		}

		public bool KeyboardVisible { get { return false; } }
		public void OnDestroy()
		{
			Application.ExternalCall("unityWebView.destroy", _name);
		}

		public void SetCenterPositionWithScale(Vector2 center, Vector2 scale)
		{
			//not implemented on web player
		}

		public void SetMargins(int left, int top, int right, int bottom)
		{
			Application.ExternalCall("unityWebView.setMargins", _name, left, top, right, bottom);
		}

		public void SetVisibility(bool v)
		{
			Application.ExternalCall("unityWebView.setVisibility", _name, v);
		}

		public void LoadUrl(string url)
		{
			Application.ExternalCall("unityWebView.loadURL", _name, GetAbsoluteUrl(url));
		}

		public void EvaluateJs(string js)
		{
			Application.ExternalCall("unityWebView.evaluateJS", _name, js);
		}

		public string GetAbsoluteUrl(string url)
		{
			if (url.StartsWith("http://"))
			{
				return url;
			}
			//Unity seems to have broken this in 4.5?
			return Application.dataPath + "/StreamingAssets/" + url;
		}

		public void AddNecessaryJavascriptEvents()
		{
			EvaluateJs(
					"parent.$(function() {" +
					"	window.Unity = {" +
					"		call:function(msg) {" +
					"			parent.unityWebView.sendMessage('WebViewObject', msg)" +
					"		}" +
					"	};" +
					"	parent.$(window).click(function() {" +
					"		window.Unity.call('clicked');" +
					"	});" +
					"});");
		}

		public void OnUpdate()
		{
			//not used for web player
		}

		public void OnGui()
		{
			//not used for web player
		}
	}
}
