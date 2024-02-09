using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;
using System.Collections.Concurrent;
using Microsoft.VisualBasic.FileIO;

namespace Game_Enhancement___C_
{
    public class Renderer : Overlay
    {
        // render variables
        // TO-DO: make this modular use the winapi: https://stackoverflow.com/a/36920187
        public Vector2 screenSize = new Vector2(1920, 1080);

        // entities copy, using more thread safe methods
        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();

        // GUI elements
        private bool enableESP = true;
        private Vector4 enemyColor = new Vector4(1, 0, 0, 1); // default red.
        private Vector4 teamColor = new Vector4(0, 1, 0, 1); // default green.

        float boneThickness = 4;

        // draw list
        ImDrawListPtr drawList;

        protected override void Render()
        {
            // ImGui menu 

            ImGui.Begin("Basic ESP");
            ImGui.Checkbox("Enable ESP", ref enableESP);

            // team color
            if (ImGui.CollapsingHeader("Team color"))
                ImGui.ColorPicker4("##teamcolor", ref teamColor);
            // enemy color
            if (ImGui.CollapsingHeader("Enemy color"))
                ImGui.ColorPicker4("##enemycolor", ref enemyColor);
            ImGui.SliderFloat("bone thickness", ref boneThickness, 4, 500);

            // draw overlay
            DrawOverlay(screenSize);
            drawList = ImGui.GetWindowDrawList();

            // draw stuff
            if (enableESP)
            {
                foreach (var entity in entities)
                {
                    // check if entity on screen
                    if (EntityOnSreen(entity))
                    {
                        // draw methods
                        DrawBox(entity);
                        DrawLine(entity);
                        DrawSkeletons();
                    }
                }
            }
            ImGui.End();

        }

        void DrawSkeletons()
        {
            // loop through the bones
            foreach (Entity entity in entities)
            {
                if (entity == null) continue;
                uint skeletonColor = localPlayer.team == entity.team ? ImGui.ColorConvertFloat4ToU32(teamColor) : ImGui.ColorConvertFloat4ToU32(enemyColor);

                // check if the head is on the screen or not
                if (entity.bones2d[2].X > 0 && entity.bones2d[2].Y > 0 && entity.bones2d[2].X < screenSize.X && entity.bones2d[2].Y < screenSize.Y)
                {
                    float currentBoneThickness = boneThickness / entity.distance;
                    // draw lines between bones
                    drawList.AddLine(entity.bones2d[1], entity.bones2d[2], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[1], entity.bones2d[2], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[1], entity.bones2d[3], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[1], entity.bones2d[6], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[3], entity.bones2d[4], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[6], entity.bones2d[7], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[4], entity.bones2d[5], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[7], entity.bones2d[8], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[1], entity.bones2d[0], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[0], entity.bones2d[9], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[0], entity.bones2d[11], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[9], entity.bones2d[10], skeletonColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2d[11], entity.bones2d[12], skeletonColor, currentBoneThickness);
                    drawList.AddCircleFilled(entity.bones2d[2], 3 + currentBoneThickness, skeletonColor);
                }

            }
        }

        // check position
        bool EntityOnSreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
                return true;

            return false;
        }

        // drawing methods

        private void DrawBox(Entity entity)
        {
            // calculate box height
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // calculate box dimensions
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);

            Vector2 rectBottom = new Vector2(entity.viewPosition2D.X + entityHeight / 3, entity.viewPosition2D.Y + entityHeight);

            // get correct color
            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
        }

        private void DrawLine(Entity entity)
        {
            // get correct color
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            // draw line
            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }


        // transfer entity methods

        public void UpdateEntities(IEnumerable<Entity> newEntities) // update entities
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }

        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }

        public Entity GetLocalPlayer() // get localPlayer
        {
            lock(entityLock);
            return localPlayer;
        }

        void DrawOverlay(Vector2 screenSize) // overlay window
        {
            Vector2 screeny = new Vector2(1920, 1080);
            ImGui.SetNextWindowSize(screeny, 0);
            ImGui.SetNextWindowPos(new Vector2(0, 0)); // in the beginning
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }
    }
}
