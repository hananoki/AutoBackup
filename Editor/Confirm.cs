#if UNITY_2019_1_OR_NEWER

using HananokiEditor.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HananokiEditor.AutoBackup {
	public class Confirm {

		static EditorWindow hierarchy => EditorWindowUtils.Find( UnityTypes.UnityEditor_SceneHierarchyWindow );
		internal IMGUIContainer s_IMGUIContainer;

		/////////////////////////////////////////
		public void Attach() {
			//if( !E.i.launchButton ) return;
			if( !UnitySymbol.UNITY_2019_1_OR_NEWER ) return;

			RemoveUI();

			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}


		/////////////////////////////////////////
		internal void RemoveUI() {
			if( s_IMGUIContainer == null ) return;
			hierarchy.RemoveIMGUIContainer( s_IMGUIContainer );
			s_IMGUIContainer = null;
			EditorWindowUtils.RepaintHierarchyWindow();
		}


		/////////////////////////////////////////
		void OnUpdate() {
			if( !DeleyAttach() ) return;

			EditorWindowUtils.RepaintHierarchyWindow();
			EditorApplication.update -= OnUpdate;
		}


		/////////////////////////////////////////
		public bool DeleyAttach() {
			if( hierarchy == null ) return false;

			s_IMGUIContainer = new IMGUIContainer( OnGUIHandler );
			hierarchy.AddIMGUIContainer( s_IMGUIContainer );

			MaxStyle();

			return true;
		}


		void MaxStyle() {
			if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
				s_IMGUIContainer.style.top = 0;
				s_IMGUIContainer.style.height = 40;
				s_IMGUIContainer.style.width = hierarchy.position.width;
				s_IMGUIContainer.style.left = 0;
				s_IMGUIContainer.style.right = 0;
				s_IMGUIContainer.style.backgroundColor = HEditorStyles.backGroundColor;
			}
			else if( UnitySymbol.UNITY_2019_2_OR_NEWER ) {
				s_IMGUIContainer.style.height = 18;
				//s_IMGUIContainer.style.width = projectBrowser.position.width - 50;
				s_IMGUIContainer.style.left = 50;
				s_IMGUIContainer.style.top = -2;
			}
			else {
				s_IMGUIContainer.style.height = 16;
				//s_IMGUIContainer.style.width = projectBrowser.position.width - 42;
				s_IMGUIContainer.style.left = 42;
				s_IMGUIContainer.style.top = -1;
				//s_IMGUIContainer.style.marginLeft = 50;
			}
		}


		/////////////////////////////////////////
		void OnGUIHandler() {
			var r = new Rect( 0, 0, hierarchy.position.width, 40 );

			ScopeArea.Begin( r );
			ScopeVertical.Begin( EditorStyles.helpBox, GUILayout.Height( 40 ) );

			ScopeHorizontal.Begin();

			GUILayout.Label( EditorHelper.TempContent( "このバックアップで元ファイルを上書きしますか？", EditorIcon.info ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			GUILayout.FlexibleSpace();
			if( GUILayout.Button( "はい", GUILayout.Height( EditorGUIUtility.singleLineHeight ) ) ) {
				EditorApplication.delayCall += () => {
					var p = SceneManager.GetSceneAt( 0 );
					EditorSceneManager.SaveScene( p, $"{Core.s_rollbackGUID.ToAssetPath()}", true );
					EditorSceneManager.OpenScene($"{Core.s_rollbackGUID.ToAssetPath()}" );
					RemoveUI();
				};
			}
			if( GUILayout.Button( "いいえ", GUILayout.Height( EditorGUIUtility.singleLineHeight ) ) ) {
				EditorApplication.delayCall += () => RemoveUI();
			}
			ScopeHorizontal.End();
			GUILayout.Label( Core.s_rollbackGUID.ToAssetPath(), EditorStyles.miniLabel, GUILayout.Height( 12 ) );

			ScopeVertical.End();
			ScopeArea.End();
		}
	}
}

#endif
