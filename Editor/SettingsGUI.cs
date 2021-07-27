#if UNITY_2019_1_OR_NEWER

using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using HananokiRuntime.Extensions;
using System;
using UnityEditor;
using UnityEngine;
using P = HananokiEditor.AutoBackup.SettingsProject;
using SS = HananokiEditor.SharedModule.S;


namespace HananokiEditor.AutoBackup {

	public class SettingsGUI {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
				mode = 1,
			};
		}



		/////////////////////////////////////////
		public static void DrawGUI() {
			P.Load();

			bool enablePackage = P.i.有効 ? P.i.m_backupPackageName.IsExistsDirectory() : false;

			ScopeChange.Begin();

			ScopeHorizontal.Begin();
			GUILayout.FlexibleSpace();
			if( enablePackage ) {
				GUILayout.Label( EditorHelper.TempContent( S._Backupfunctionisworking, EditorIcon.lightmeter_greenlight ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			}
			else {
				GUILayout.Label( EditorHelper.TempContent( S._Backupfunctionhasstoppedworking, EditorIcon.lightmeter_lightrim ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			}
			ScopeHorizontal.End();
			GUILayout.Space( 4 );


			/////////////////////////////////////////
			HEditorGUILayout.HeaderTitle( S._Backuppackage );

			ScopeHorizontal.Begin();

			ScopeDisable.Begin( enablePackage );
			var cont = EditorHelper.TempContent( S._PackageName );
			var rc = GUILayoutUtility.GetRect( cont, EditorStyles.textField );
			rc = EditorGUI.PrefixLabel( rc, cont );
			P.i.m_backupPackageName = EditorGUI.TextField( rc, P.i.m_backupPackageName );

			if( P.i.m_backupPackageName.IsEmpty() && !EditorHelper.IsActiveTextField( rc ) ) {
				ScopeDisable.Begin( true );
				EditorGUI.LabelField( rc.TrimR( 2 ).TrimL( 4 ), S._Usedforthedisplaynameofthepackageandthenameoftheoutputfolder, EditorStyles.miniLabel );
				ScopeDisable.End();
			}

			ScopeDisable.End();

			//GUILayout.FlexibleSpace();
			if( enablePackage ) {
				if( GUILayout.Button( SS._Disable, GUILayout.ExpandWidth( false ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) ) ) {
					P.i.有効 = false;
					Core.Exit();
					try {
						PackageDatabaseUtils.RemovePackage( P.i.m_backupPackageName );
					}
					catch( Exception ) {
						EditorUtility.DisplayDialog( SS._Info, $"{S._Stoppedworking}\n{S._DisablethebackuppackagefromPackageManager}", SS._OK );
						EditorApplication.ExecuteMenuItem( "Window/Package Manager" );
					}
				}
			}
			else {
				if( GUILayout.Button( SS._Enable, GUILayout.ExpandWidth( false ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) ) ) {
					if( P.i.m_backupPackageName.IsEmpty() ) {
						EditorUtility.DisplayDialog( SS._Info, $"No package name entered", SS._OK );
					}
					else {
						Utils.バックアップパッケージをインストールする();
						EditorApplication.update -= Utils.Install完了時の処理;
						EditorApplication.update += Utils.Install完了時の処理;
					}
				}
			}

			ScopeHorizontal.End();

			GUILayout.Space( 4 );


			/////////////////////////////////////////
			HEditorGUILayout.HeaderTitle( SS._Option );

			EditorGUILayout.LabelField( S._Backupinterval_minutes_ );
			EditorGUI.indentLevel++;
			P.i.m_BackupInterval_Minutes = EditorGUILayout.IntSlider( P.i.m_BackupInterval_Minutes, 1, 60 );
			EditorGUI.indentLevel--;

			P.i.Backupscenesを退避する = HEditorGUILayout.ToggleLeft( S._Save__Backupscenesatstartofplaymode, P.i.Backupscenesを退避する );

			P.i.バックアップ時にSaveAssetsを呼ぶ = HEditorGUILayout.ToggleLeft( S._CallSaveAssetswhenbackingup, P.i.バックアップ時にSaveAssetsを呼ぶ );
			P.i.指定ファイル数を超えたら古いファイルを消す = HEditorGUILayout.ToggleLeft( S._Deleteoldfileswhenthespecifiednumberoffilesisexceeded, P.i.指定ファイル数を超えたら古いファイルを消す );
			ScopeDisable.Begin( !P.i.指定ファイル数を超えたら古いファイルを消す );
			EditorGUI.indentLevel++;
			P.i.m_maxFileCount = EditorGUILayout.IntSlider( P.i.m_maxFileCount, 1, 100 );
			EditorGUI.indentLevel--;
			ScopeDisable.End();

			if( ScopeChange.End() ) {
				//s_changed = true;
				P.Save();
			}

			GUILayout.Space( 4 );


			/////////////////////////////////////////
			HEditorGUILayout.HeaderTitle( SS._Command );

			if( GUILayout.Button( S._Backupyourscenenow ) ) {
				Utils.シーンをバックアップ( true );
			}
			if( GUILayout.Button( S._Cleanupbackup ) ) {
				Utils.バックアップをクリーン();
			}

		}
	}
}

#endif
