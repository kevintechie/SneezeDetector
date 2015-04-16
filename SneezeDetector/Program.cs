using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace SneezeDetector
{
	public class Program
	{
		protected static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
		protected static InputPort mic = new InputPort(Pins.GPIO_PIN_D3, true, Port.ResistorMode.Disabled);
		protected static Boolean sneezeDetected = false;

		// NTP Server
		protected static string NTPServer = "us.pool.ntp.org";
		protected static double UtcOffsetMinutes = -480;

		// SuperTweet
		protected static string userID = "gabudomain";
		protected static string password = "doorman01";
		protected static SuperTweet twitterClient = null;
		
		protected static string[] messages = new string[] { "We have a winner.", "That's how to do it.", "She's breaking up, Captain!", "Whoop. There it is.", "Bam!" };
		protected static Random random = new Random();
		protected static bool timeUpdated = false;

		protected static bool IsDebug
		{
			get
			{
				bool debug = false;
#if DEBUG
            debug = true;
#endif
				return debug;
			}
		}

		public static void Main()
		{
			// Update the clock in a separate thread
			Thread clockThread = new Thread(UpdateTime);
			Debug.Print("Updating date/time...");
			clockThread.Start();

			// Countdown to abort the clock update thread if the NTP server cannot be reached
			int countdown = 20;

			// Blink the blue LED while the time is updating
			while (clockThread.IsAlive)
			{
				led.Write(true);
				Thread.Sleep(200);
				led.Write(false);
				Thread.Sleep(200);
				if (countdown-- < 0)
					clockThread.Abort();
			}

			// If the time update was successful, set up the TweetBot
			if (timeUpdated)
			{
				led.Write(false);
				twitterClient = new SuperTweet(userID, password);
			}

			// Time update successful, print the current date/time to the debug console
			Debug.Print("Current date/time: " + DateTime.Now.AddMinutes(UtcOffsetMinutes).ToString());

			while (true)
			{

				sneezeDetected = !mic.Read();
				if (sneezeDetected)
				{
					int rnd = random.Next(4);
					string message = messages[rnd] + " An epic sneeze detected at " + DateTime.Now.AddMinutes(UtcOffsetMinutes).ToString("h:mm:ss tt") + " by the POP automated sneeze tweeter.";

					if (IsDebug)
					{
						Debug.Print(message);
					}
					else
					{
						try
						{
							twitterClient.Tweet(message);
						}
						catch (Exception e)
						{
							Debug.Print(e.ToString());
						}
					}

					//Blink the onboard LED to confirm tweet was sent.
					led.Write(true);
					Thread.Sleep(500);
					led.Write(false);

				}

			}

		}

		static void UpdateTime()
		{
			timeUpdated = false;
			timeUpdated = NTP.UpdateTimeFromNTPServer(NTPServer);
		}

	}
}
