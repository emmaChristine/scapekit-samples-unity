using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
    internal class DocsWindow : EditorWindow
    {
        WebViewHook webView;
        string url = "https://developer-docs.scape.io/site/unity/requirements/";

        [MenuItem("ScapeKit/Docs %#d", false, 3)]
        static void Load()
        {
            DocsWindow window = GetWindow<DocsWindow>("ScapeKit Docs");
            window.Show();
        }

        void OnEnable()
        {
            if (!webView)
            {
                // create webView
                webView = CreateInstance<WebViewHook>();
            }
        }

        public void OnBecameInvisible()
        {
            if (webView)
            {
                // signal the browser to unhook
                webView.Detach();
            }
        }

        void OnDestroy()
        {
            //Destroy web view
            DestroyImmediate(webView);
        }

        void OnGUI()
        {
            // hook to this window
            if (webView.Hook(this))
                // do the first thing to do
                webView.LoadURL(url);

            var ev = Event.current;
            if (ev.type == EventType.Repaint)
            {
                // keep the browser aware with resize
                webView.OnGUI(new Rect(0, 20, position.width, position.height - 20));
            }
        }
    }
}
