using UnityEngine;

public static class NetworkUtil
{
    private static bool? _hasInternetConnection;

    public static readonly bool HasInternetConnectionCached = _hasInternetConnection ??= CheckHasInternetConnection();

    public static bool HasInternetConnection => CheckHasInternetConnection();

    public static bool CheckHasInternetConnection(bool showLog = true)
    {
        NetworkReachability reachability = Application.internetReachability;

        switch (reachability)
        {
            case NetworkReachability.NotReachable:
                if (showLog)
                    Debug.Log("Device is not connected to the internet");
                return false;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                if (showLog)
                    Debug.Log("Device is connected to the internet via mobile data network");
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                if (showLog)
                    Debug.Log("Device is connected to the internet via Wi-Fi or Ethernet");
                break;
        }

        return true;
    }
}