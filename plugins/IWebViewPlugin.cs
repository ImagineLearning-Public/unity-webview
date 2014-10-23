using UnityEngine;

namespace Assets.Plugins
{
	public interface IWebViewPlugin
	{
		void Init(string name);
		bool KeyboardVisible { get;}
		void OnDestroy();
		void SetCenterPositionWithScale(Vector2 center, Vector2 scale);
		void SetMargins(int left, int top, int right, int bottom);
		void SetVisibility(bool v);
		void LoadUrl(string url);
		void EvaluateJs(string js);
		string GetAbsoluteUrl(string url);
		void AddNecessaryJavascriptEvents();
		void OnUpdate();
		void OnGui();
	}
}
