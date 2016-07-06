using UnityEngine.Networking;

/// <summary>
/// The message class we use to encapsulate our json in UNET
/// </summary>
public class jsonStringMessage : MessageBase {
	public string json;
}
