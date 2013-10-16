/******************************************************************************
 * Spine Runtime Software License - Version 1.1
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms in whole or in part, with
 * or without modification, are permitted provided that the following conditions
 * are met:
 * 
 * 1. A Spine Essential, Professional, Enterprise, or Education License must
 *    be purchased from Esoteric Software and the license must remain valid:
 *    http://esotericsoftware.com/
 * 2. Redistributions of source code must retain this license, which is the
 *    above copyright notice, this declaration of conditions and the following
 *    disclaimer.
 * 3. Redistributions in binary form must reproduce this license, which is the
 *    above copyright notice, this declaration of conditions and the following
 *    disclaimer, in the documentation and/or other materials provided with the
 *    distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Spine;

namespace SpineExample {
	public class Example : Microsoft.Xna.Framework.Game {
		GraphicsDeviceManager graphics;
		
        SkeletonRenderer skeletonRenderer;
		Skeleton skeleton;
        Skeleton skeleton2;
		Slot headSlot;
		AnimationState state;
        AnimationState state2;
        SkeletonBounds bounds;
        SkeletonBounds bounds2;

       
        Camera2D camera;
        
		public Example () {
			IsMouseVisible = true;

			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = 640;
			graphics.PreferredBackBufferHeight = 480;
		}

		protected override void Initialize () {
			// TODO: Add your initialization logic here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(1.0f);
			base.Initialize();
		}

		protected override void LoadContent () {
            
            TouchPanel.EnabledGestures = GestureType.None;
			skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
			skeletonRenderer.PremultipliedAlpha = true;
            camera = new Camera2D(GraphicsDevice);
            Vector2 screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            camera.Position = screenCenter;
            Vector2 pos1 = new Vector2(graphics.GraphicsDevice.Viewport.Width / 3, graphics.GraphicsDevice.Viewport.Height * 2 / 3);
			String name = "spineboy"; // "goblins";
            AnimationStateData stateData;
            LoadFigure(name, pos1, out skeleton, out stateData, out bounds);
//			Atlas atlas = new Atlas("Content/" + name + ".atlas", new XnaTextureLoader(GraphicsDevice));
//			SkeletonJson json = new SkeletonJson(atlas);
//			skeleton = new Skeleton(json.ReadSkeletonData("Content/" + name + ".json"));
//			if (name == "goblins") skeleton.SetSkin("goblingirl");
//			skeleton.SetSlotsToSetupPose(); // Without this the skin attachments won't be attached. See SetSkin.

			// Define mixing between animations.
//			AnimationStateData stateData = new AnimationStateData(skeleton.Data);
			if (name == "spineboy") {
				stateData.SetMix("walk", "jump", 0.2f);
				stateData.SetMix("jump", "walk", 0.4f);
			}

			state = new AnimationState(stateData);

			if (true) {
				// Event handling for all animations.
				state.Start += new EventHandler<StartEndArgs>(Start);
				state.End += new EventHandler<StartEndArgs>(End);
				state.Complete += new EventHandler<CompleteArgs>(Complete);
				state.Event += new EventHandler<EventTriggeredArgs>(Event);

                state.SetAnimation(0, "walk", true); // drawOrder
			} else {
				state.SetAnimation(0, "walk", false);
				TrackEntry entry = state.AddAnimation(0, "jump", false, 0);
				entry.End += new EventHandler<StartEndArgs>(End); // Event handling for queued animations.
				state.AddAnimation(0, "walk", true, 0);
			}

            name = "goblins"; // ;
            AnimationStateData stateData2;
            Vector2 pos2 = new Vector2(graphics.GraphicsDevice.Viewport.Width * 2 / 3, graphics.GraphicsDevice.Viewport.Height * 2 / 3);
            LoadFigure(name, pos2, out skeleton2, out stateData2, out bounds2);
            skeleton2.SetSkin("goblingirl");
            skeleton2.SetSlotsToSetupPose();

            state2 = new AnimationState(stateData2);

			// Event handling for all animations.
			state2.Start += new EventHandler<StartEndArgs>(Start);
			state2.End += new EventHandler<StartEndArgs>(End);
			state2.Complete += new EventHandler<CompleteArgs>(Complete);
			state2.Event += new EventHandler<EventTriggeredArgs>(Event);

            state2.SetAnimation(0, "walk", true); // drawOrder


            /*
            skeleton.X = screenCenter.X; //320;
            skeleton.Y = screenCenter.Y; //440;
			skeleton.UpdateWorldTransform();
            bounds.Update(skeleton, true);
            float height = bounds.MaxY - bounds.MinY;
            skeleton.Y += height;
            skeleton.UpdateWorldTransform();
            */ 
			headSlot = skeleton.FindSlot("head");
             
		}

		protected override void UnloadContent () {
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update (GameTime gameTime) {
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();
            
			// TODO: Add your update logic here
            camera.Update(gameTime);
			base.Update(gameTime);
             
		}

		protected override void Draw (GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
            
			state.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
			state.Apply(skeleton);
			skeleton.UpdateWorldTransform();

            state2.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            state2.Apply(skeleton2);
            skeleton2.UpdateWorldTransform();

            skeleton.FlipX = true;
            skeletonRenderer.Begin(ref camera.View); // (camera.View);
			skeletonRenderer.Draw(skeleton);
            skeletonRenderer.Draw(skeleton2);
			skeletonRenderer.End();

			bounds.Update(skeleton, true);
            TouchCollection touchCollection = TouchPanel.GetState();
            TouchLocation tl = touchCollection.FirstOrDefault();
            
                
			headSlot.G = 1;
			headSlot.B = 1;
            if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
            {
                if (bounds.AabbContainsPoint(tl.Position.X, tl.Position.Y))
                {
                    BoundingBoxAttachment hit = bounds.ContainsPoint(tl.Position.X, tl.Position.Y);
                    if (hit != null)
                    {
                        headSlot.G = 0;
                        headSlot.B = 0;
                    }
                }
            }
            
			base.Draw(gameTime);
		}
        
		public void Start (object sender, StartEndArgs e) {
			Console.WriteLine(e.TrackIndex + " " + state.GetCurrent(e.TrackIndex) + ": start");
		}

		public void End (object sender, StartEndArgs e) {
			Console.WriteLine(e.TrackIndex + " " + state.GetCurrent(e.TrackIndex) + ": end");
		}

		public void Complete (object sender, CompleteArgs e) {
			Console.WriteLine(e.TrackIndex + " " + state.GetCurrent(e.TrackIndex) + ": complete " + e.LoopCount);
		}

		public void Event (object sender, EventTriggeredArgs e) {
			Console.WriteLine(e.TrackIndex + " " + state.GetCurrent(e.TrackIndex) + ": event " + e.Event);
		}

        private void LoadFigure(String name, Vector2 pos, out Skeleton skel, out AnimationStateData animationData, out SkeletonBounds sbounds)
        {
            Atlas atlas = new Atlas("Content/" + name + ".atlas", new XnaTextureLoader(GraphicsDevice));
            SkeletonJson json = new SkeletonJson(atlas);
            skel = new Skeleton(json.ReadSkeletonData("Content/" + name + ".json"));
            animationData = new AnimationStateData(skel.Data);

            skel.X = pos.X; //320;
            skel.Y = pos.Y; //440;
 //           skel.UpdateWorldTransform();
            sbounds = new SkeletonBounds();
 //           sbounds.Update(skel, true);
 //           float height = sbounds.MaxY - sbounds.MinY;
 //           skel.Y += height;
 //           skel.UpdateWorldTransform();
        }
	}
}
