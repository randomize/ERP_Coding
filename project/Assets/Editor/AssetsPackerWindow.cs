﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ZFrame.Asset;
using Assets.Editor.Utils;

public class AssetsPackerWindow : EditorWindow {

    [MenuItem("Custom/资源打包...")]
    static void OpenWindow()
    {
		EditorWindow.GetWindowWithRect(typeof(AssetsPackerWindow), new Rect(10, 10, 220, 400), true, "资源打包");
    }

	private Vector2 BtnSizeLV1 = new Vector2(200, 50);
	private Vector2 BtnSizeLV2 = new Vector2(180, 30);
	private Vector2 BtnSizeLV3 = new Vector2(100, 25);

	private void PackAssets()
	{
		AssetPacker.EncryptLua();
		AssetPacker.PackAssets();
		AssetPacker.GenFileList();
	}

	public static void UpdateAssets()
	{
		var srcDir = AssetPacker.EditorStreamingAssetsPath;
		var dstDir = AssetPacker.StreamingAssetsPath;
		
		AssetPacker.Log("Copying {0} -> {1}", srcDir, dstDir);
		
		AssetPacker.ClearStreamingAssets();
		
		SystemTools.CopyDirectory(srcDir, dstDir, "*.unity3d");
		
		File.Copy(srcDir + "/filelist.bytes", dstDir + "/filelist.bytes", true);
		
		AssetPacker.ClearEditorPersistentAssets();
        
        AssetDatabase.Refresh();
        
        AssetPacker.Log("Copying Done");
    }

	private void BuildApp()
	{	
		const string fileName = "TSK";
		
#if UNITY_STANDALONE_WIN
		const string FolderName = "PC";
		const string packName = fileName + ".exe";
#elif UNITY_STANDALONE_OSX
		const string FolderName = "MAC";
		const string packName = fileName + ".app";
#elif UNITY_ANDROID
		const string FolderName = "AND";
		const string packName = fileName + ".apk";
#elif UNITY_IOS
		const string FolderName = "IOS";
		const string packName = fileName + ".ipa";
#endif
		
#if RY_DEBUG
		const BuildOptions bo = BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
#else
		const BuildOptions bo = BuildOptions.None;
#endif
		//var verInfo = VersionMgr.LoadAppVersion();
        VersionMgr.SaveAppVersion(GitTools.getVerInfo());
		AssetDatabase.Refresh();
		
		//清空PC执行文件目录的缓存文件。
		string ProductPath = Application.dataPath + "/../../Products";
		string ProductPCPath = Path.Combine(ProductPath, FolderName);
		string BuildName = ProductPCPath + "/" + packName;
		
		if (!Directory.Exists(ProductPath)) Directory.CreateDirectory(ProductPath);
		if (!Directory.Exists(ProductPCPath)) Directory.CreateDirectory(ProductPCPath);

        //开始打游戏包
        Debug.Log(BuildName);
		BuildPipeline.BuildPlayer(new string[] {
			"Assets/Scenes/ZERO.unity",
		}, BuildName, AssetPacker.buildTarget, bo);
		
#if UNITY_STANDALONE
		AssetPacker.Log("Coping Essets ...");
		SystemTools.CopyDirectory(Application.dataPath + "/../Essets", ProductPCPath + "/Essets");
#endif
		
#if UNITY_IOS
		XCodePostProcess.OnPostProcessBuild(BuildName);
#endif
		
		AssetPacker.Log("Build Done: " + BuildName);
	}

	private void PackAndBuild()
	{
		AssetsOperation.RemoveUnusedAssest();
		PackAssets();
		UpdateAssets();
		BuildApp();
	}

	private void ClearAssets()
	{
		if (EditorUtility.DisplayDialog(
			"注意", 
			"清空已生成的资源包缓存后，\n之后生成资源包操作将重新生成所有资源。", 
			"确定", "取消")) {
			AssetPacker.ClearEditorStreamingAssets();
		}
	}

    private void EncryptLua()
    {
        AssetPacker.EncryptLua();
        AssetDatabase.Refresh();
    }

	private void VERT_Btn(ref int vertOffset, int horiOffset, Vector2 size, string title, string tooltip, System.Action funcPack)
	{
		if (GUI.Button(
			new Rect(horiOffset, vertOffset, size.x, size.y), 
			new GUIContent(title, tooltip), 
			CustomEditorStyles.richTextBtn)) {
			funcPack.Invoke();
		}
		
		vertOffset += (int)size.y;
		vertOffset += 5;
	}

	private void HORI_Btn(ref int horiOffset, int vertOffset, Vector2 size, string title, System.Action funcPack)
	{
		if (GUI.Button(new Rect(horiOffset, vertOffset, size.x, size.y), title, CustomEditorStyles.richTextBtn)) {
			funcPack.Invoke();
		}
		
		horiOffset += (int)size.x;
		horiOffset += 5;
	}

    public void OnGUI()
    {
		int vertOffset = 10;
		VERT_Btn(ref vertOffset, 10, BtnSizeLV1, "<size=40>一键出包</size>", "", PackAndBuild);
		VERT_Btn(ref vertOffset, 20, BtnSizeLV2, "<size=20>1. 删除废弃资源包</size>", "", AssetsOperation.RemoveUnusedAssest);
		VERT_Btn(ref vertOffset, 20, BtnSizeLV2, "<size=20>2. 生成资源包*</size>", "先加密Lua脚本\n再打包有变化的资源\n最后重新生成filelist文件", PackAssets);
		VERT_Btn(ref vertOffset, 20, BtnSizeLV2, "<size=20>3. 更新资源包*</size>", "把生成的所有资源包拷贝到StreamingAssets目录", UpdateAssets);
		VERT_Btn(ref vertOffset, 20, BtnSizeLV2, "<size=20>4. 生成应用*</size>", "使用已生成的资源包，直接执行生成应用", BuildApp);

        vertOffset += 30;
		GUI.Label(new Rect(0, vertOffset, BtnSizeLV2.x, BtnSizeLV2.y), "编辑器/调试专用：", CustomEditorStyles.richText);
        vertOffset += 20;

        VERT_Btn(ref vertOffset, 10, BtnSizeLV3, "加密Lua", "", EncryptLua);
		VERT_Btn(ref vertOffset, 10, BtnSizeLV3, "资源打包", "", AssetPacker.PackAssets);
		VERT_Btn(ref vertOffset, 10, BtnSizeLV3, "更新filelist", "", AssetPacker.UpdateFileList);

		vertOffset += 20;
		VERT_Btn(ref vertOffset, 20, BtnSizeLV2, "<color=red><size=20>清除资源包</size></color>", "", ClearAssets);
    }
    
}
