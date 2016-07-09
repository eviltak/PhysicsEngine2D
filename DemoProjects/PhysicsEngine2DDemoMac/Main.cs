#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using MonoMac.AppKit;
using MonoMac.Foundation;
using Microsoft.Xna.Framework;
using System.Drawing;

using PhysicsEngine2DDemo.Demos;
#endregion

namespace PhysicsEngine2DDemoMac
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			NSApplication.Init();

			NSApplication application = NSApplication.SharedApplication;
			application.Delegate = new AppDelegate();
			application.Run();
		}
	}

	public class AppDelegate : NSApplicationDelegate
	{
		DemoGame game;
		public override void DidFinishLaunching(NSNotification notification)
		{
			game = new DemoGame();
			game.Run();
		}
	}
}


