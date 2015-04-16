using System;
using Microsoft.SPOT;
using Toolbox.NETMF;
using Toolbox.NETMF.NET;

namespace SneezeDetector
{
	public class SuperTweet
	{
		protected readonly string userId;
		protected readonly string password;

		public SuperTweet(string userId, string password)
		{
			this.userId = userId;
			this.password = password;
		}

		public bool Tweet(string message)
		{
			HTTP_Client WebSession = new HTTP_Client(new IntegratedSocket("api.supertweet.net", 80));
			WebSession.Authenticate(userId, password);
			HTTP_Client.HTTP_Response Response = WebSession.Post("/1.1/statuses/update.json", "status=" + Tools.RawUrlEncode(message) + "&trim_user=true");

			if (Response.ResponseCode != 200)
			{
				Debug.Print(Response.ToString());
				throw new ApplicationException("Unexpected HTTP response code: " + Response.ResponseCode.ToString());
			}

			//Debug.Print(Response.ToString());
			return true;
		}
	}
}
