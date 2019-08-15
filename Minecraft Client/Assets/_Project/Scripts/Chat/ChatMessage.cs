﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ChatMessage
{
	/// <summary>
	/// Gets the message with no styling information
	/// </summary>
	public string PlaintextMessage { get; set; }
	
	/// <summary>
	/// For use in TextMeshPro dialogs
	/// </summary>
	public string  HtmlFormattedMessage { get; set; }

	public Position ChatPosition { get; set; }

	public ChatMessage(ChatMessagePacket pkt)
	{
		var chatComponent = ChatComponent.FromJson(pkt.Json);

		PlaintextMessage = chatComponent.GetComponentText(false, new LinkedList<string>());
	}

	public enum Position : byte
	{
		CHAT = 0x00,
		SYSTEM_MESSAGE = 0x01,
		ABOVE_HOTBAR = 0x02
	}
}
