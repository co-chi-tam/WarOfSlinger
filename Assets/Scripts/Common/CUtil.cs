using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CUtil {

	public static string GetMd5Hash(this string input)
	{
		// Create Hash
		MD5 md5Hash = MD5.Create ();

		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}

	// Verify a hash against a string.
	static bool VerifyMd5Hash(this string input, string hash)
	{
		// Hash the input.
		string hashOfInput = input.GetMd5Hash();

		// Create a StringComparer an compare the hashes.
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;

		if (0 == comparer.Compare(hashOfInput, hash))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static Sprite FindResourceSprite(string name) {
		var resourceSprites = Resources.LoadAll<Sprite> ("Image");
		for (int i = 0; i < resourceSprites.Length; i++) {
			var spriteObj = resourceSprites [i];
			if (spriteObj.name.Equals (name)) {
				return spriteObj;
			}
		}
		return null;
	}

	public static Vector3 ToV3(this string value) {
		var resultV3 = Vector3.zero;
		value = value.Replace ("(", "").Replace(")", ""); // (x,y,z)
		var splits = value.Split (','); // x,y,z
		resultV3.x = float.Parse (splits[0].ToString());
		resultV3.y = float.Parse (splits[1].ToString());
		resultV3.z = float.Parse (splits[2].ToString());
		return resultV3;
	}

	public static string ToV3String (this Vector3 value) {
		var result = new StringBuilder ("(");
		result.Append (value.x + ",");
		result.Append (value.y + ",");
		result.Append (value.z + "");
		result.Append (")");
		return result.ToString();
	}

	public static bool IsPointerOverUIObject(Vector2 position) {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = position;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public static Vector3 GetCenterScreen2WorldPoint() {
		var halfWidth = (float)Screen.width / 2f;
		var halfHeight = (float)Screen.height / 2f;
		return Camera.main.ScreenToWorldPoint (new Vector3 (halfWidth, halfHeight, 0f));
	}

}
