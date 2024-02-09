using Game_Enhancement___C_;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Numerics;
using System.Runtime.InteropServices;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// get the keystroke so that we can check if user wants to quit
[DllImport("user32.dll")]
static extern bool GetAsyncKeyState(int vKey);

// main logic
MemManager memManage = new MemManager("cs2");
Reader reader = new Reader(memManage);

// get client module
IntPtr client = memManage.GetModuleBase("client.dll");

// init render
Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

// get screen size from renderer
Vector2 screenSize = renderer.screenSize;

// store entities
List<Entity> entities = new List<Entity>();
Entity localPlayer = new Entity();

// offsets (can be gotten from: https://github.com/a2x/cs2-dumper)

// offsets.cs
int dwEntityList = 0x18B0FC8;
int dwViewMatrix = 0x19102B0;
int dwLocalPlayerPawn = 0x17262E8;

// client.dll.cs
int m_vOldOrigin = 0x127C;
int m_iTeamNum = 0x3CB;
int m_lifeState = 0x338;
int m_hPlayerPawn = 0x7E4;
int m_vecViewOffset = 0xC58;
int m_modelState = 0x160;
int m_pGameSceneNode = 0x318;

// ESP loop
while (true)
{
    if (GetAsyncKeyState(0x1B))
    {
        Environment.Exit(1);
    }
    entities.Clear(); // clean list

    IntPtr entityList = memManage.ReadPointer(client, dwEntityList);

    // make entry
    IntPtr listEntry = memManage.ReadPointer(entityList, 0x10);

    // get localplayer
    IntPtr localPlayerPawn = memManage.ReadPointer(client, dwLocalPlayerPawn);

    // get team number
    localPlayer.team = memManage.ReadInt(localPlayerPawn, m_iTeamNum);
    localPlayer.position = memManage.ReadVec(localPlayerPawn, m_vOldOrigin);

    // loop through entity list
    for (int i = 0; i < 64; i++)
    {
        // get current controller
        IntPtr currentController = memManage.ReadPointer(listEntry, i * 0x78);

        if (currentController == IntPtr.Zero) continue; // check if entity exists

        // get pawn handle
        int pawnHandle = memManage.ReadInt(currentController, m_hPlayerPawn);

        if (pawnHandle == 0) continue;

        // get current pawn, make second entry
        IntPtr listEntry2 = memManage.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF)  >> 9) + 0x10);
        if (listEntry2 == IntPtr.Zero) continue;

        // get current pawn;
        IntPtr currentPawn = memManage.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == IntPtr.Zero) continue;
        // make sure we don't draw ESP for ourselves
        if (currentPawn == localPlayerPawn) continue;

        IntPtr sceneNode = memManage.ReadPointer(currentPawn, m_pGameSceneNode);
        // 0x80 is dwBoneMatrix
        IntPtr boneMatrix = memManage.ReadPointer(sceneNode, m_modelState + 0x80);

        // check if entity is alive
        int lifeState = memManage.ReadInt(currentPawn, m_lifeState);
        if (lifeState != 256) continue;

        // get matrix
        float[] viewMatrix = memManage.ReadMatrix(client + dwViewMatrix);

        // populate entity
        Entity entity = new Entity();
        entity.team = memManage.ReadInt(currentPawn, m_iTeamNum);
        entity.position = memManage.ReadVec(currentPawn, m_vOldOrigin);
        entity.viewOffset = memManage.ReadVec(currentPawn, m_vecViewOffset);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);
        entity.distance = Vector3.Distance(entity.position, localPlayer.position);
        entity.bones = reader.ReadBones(boneMatrix);
        entity.bones2d = reader.ReadBones2D(entity.bones, viewMatrix, screenSize);

        entities.Add(entity);
    }

    // update renderer
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateEntities(entities);

}
