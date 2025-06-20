using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildVersionProcesor : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();

        switch(report.summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
                buildScriptableObject.BuildNumber = PlayerSettings.macOS.buildNumber;
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
                buildScriptableObject.BuildNumber = PlayerSettings.iOS.buildNumber;
                break;
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode++;
                buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
            case BuildTarget.PS4:
                PlayerSettings.PS4.appVersion = IncrementBuildNumber(PlayerSettings.PS4.appVersion);
                buildScriptableObject.BuildNumber = PlayerSettings.PS4.appVersion;
                break;
            case BuildTarget.WSAPlayer:
                PlayerSettings.WSA.packageVersion = new(PlayerSettings.WSA.packageVersion.Major, PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);
                buildScriptableObject.BuildNumber = PlayerSettings.WSA.packageVersion.Build.ToString();
                break;
                /*
            case BuildTarget.Switch:
                PlayerSettings.Switch.displayVersion = IncrementBuildNumber(PlayerSettings.Switch.displayVersion);
                PlayerSettings.Switch.releaseVersion = IncrementBuildNumber(PlayerSettings.Switch.releaseVersion);
                // which one to use?
                buildScriptableObject.BuildNumber = PlayerSettings.Switch.displayVersion;
                break;
                */
            case BuildTarget.tvOS:
                PlayerSettings.tvOS.buildNumber = IncrementBuildNumber(PlayerSettings.tvOS.buildNumber);
                buildScriptableObject.BuildNumber = PlayerSettings.tvOS.buildNumber;
                break;
        }

        AssetDatabase.DeleteAsset("Assets/Resources/EditorUtilities/Build.asset"); // delete any old build.asset
        // create the new one with correct build number before build starts
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/EditorUtilities/Build.asset");
        AssetDatabase.SaveAssets();
    }

    private static string IncrementBuildNumber(string buildNumber)
    {
        int.TryParse(buildNumber, out int outputBuildNumber);

        return (outputBuildNumber + 1).ToString();
    }
}
