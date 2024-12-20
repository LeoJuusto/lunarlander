
using System.Numerics; // Vector2 tarvitaan
using ZeroElectric.Vinculum;

namespace LunarLander
{
    internal class Lander
    {
        static void Main(string[] args)
        {
            Lander game = new Lander();
            game.Init();
            game.GameLoop();
        }

        /////////////////////////////////////

        // Pelaajan sijainti
        float x = 120;
        float y = 16;

        // Onko moottori päällä
        bool engine_on = false;

        // Pelaajan nopeus, polttoaine ja polttonopeus
        float velocity = 0;
        float fuel = 100;
        float fuel_consumption = 10.0f;

        // Laskeutumisalustan katon sijainti y-akselilla. Y kasvaa alaspäin.
        int landing_y = 125;

        // Ruudunpäivitykseen menevä aika (oletus)
        float delta_time = 1.0f / 60.0f;

        // Moottorin voimakkuus
        float acceleration = 20.9f;

        // Painovoiman voimakkuus
        float gravity = 9.89f;

        // Pelialueen ja ikkunan mittasuhteet
        int game_width = 240;
        int game_height = 136;

        int screen_width = 1280;
        int screen_height = 720;

        RenderTexture2D gameTexture;

        void Init()
        {
            // Aloita Raylib ja luo ikkuna.
            Raylib.InitWindow(screen_width, screen_height, "Lunar Lander");
            Raylib.SetTargetFPS(60);

            // Luo tekstuuri, johon pelialue piirretään
            gameTexture = Raylib.LoadRenderTexture(game_width, game_height);
        }

        void GameLoop()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            // Sulje Raylib-ikkuna
            Raylib.CloseWindow();
        }

        void Update()
        {
            // Päivitä delta_time
            delta_time = Raylib.GetFrameTime();

            // Lisää painovoiman vaikutus
            velocity += gravity * delta_time;

            // Tarkista, painaako pelaaja "ylös" ja onko polttoainetta jäljellä
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP) && fuel > 0)
            {
                velocity -= acceleration * delta_time;
                fuel -= fuel_consumption * delta_time;
                engine_on = true;
            }
            else
            {
                engine_on = false;
            }

            // Liikuta alusta
            y += velocity * delta_time;

            // Varmista, ettei aluksen y-arvo ylitä pelialueen rajoja
            if (y > game_height) y = game_height;
            if (y < 0) y = 0;
        }

        void Draw()
        {
            // Aloita pelialueen piirtäminen tekstuuriin
            Raylib.BeginTextureMode(gameTexture);
            Raylib.ClearBackground(Raylib.BLACK);

            // Piirrä laskeutumisalusta
            int plat_x = (int)x - 30;
            int plat_y = landing_y;
            int plat_w = 60;
            int plat_h = 10;
            Raylib.DrawRectangle(plat_x, plat_y, plat_w, plat_h, Raylib.GREEN);

            // Piirrä alus
            Raylib.DrawTriangle(
                new Vector2(x, y - 30),
                new Vector2(x - 10, y),
                new Vector2(x + 10, y),
                Raylib.SKYBLUE
            );

            // Piirrä moottorin liekki
            if (engine_on)
            {
                Raylib.DrawTriangle(
                    new Vector2(x - 5, y),
                    new Vector2(x, y + 32),
                    new Vector2(x + 5, y),
                    Raylib.YELLOW
                );
            }

            // Piirrä polttoaineen tilanne
            Raylib.DrawRectangle(9, 9, 102, 12, Raylib.BLUE);
            Raylib.DrawRectangle(10, 10, (int)fuel, 10, Raylib.YELLOW);
            Raylib.DrawText("FUEL", 11, 11, 12, Raylib.DARKBLUE);

            // Piirrä debug-tiedot
            Raylib.DrawText($"V:{velocity:F2}", 11, 31, 8, Raylib.WHITE);
            Raylib.DrawText($"dt:{delta_time:F4}", 11, 41, 8, Raylib.WHITE);

            Raylib.EndTextureMode();

            // Skaalaa pelialue koko ruutuun
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.DrawTexturePro(
                gameTexture.texture,
                new Rectangle(0, 0, game_width, -game_height),
                new Rectangle(0, 0, screen_width, screen_height),
                new Vector2(0, 0),
                0.0f,
                Raylib.WHITE
            );
            Raylib.EndDrawing();
        }
    }
}
