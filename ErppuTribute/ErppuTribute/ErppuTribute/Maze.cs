﻿/****************************************************
 * Class: Maze
 * Description: Business logic for building the maze
 * Author(s): Matti Jokitulppo, Jonah Ahvonen
 * Date: April 1, 2014
****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace ErppuTribute
{
    class Maze
    {
        #region Fields

        public const int mazeWidth = 25;
        public const int mazeHeight = 25;
        public Vector3 fogColor = Color.Black.ToVector3();
        public MazeCell[,] MazeCells = new MazeCell[mazeWidth, mazeHeight];
        public Texture2D normalEero;
        public Texture2D scaryEero;
        public Texture2D drawTexture;
        GraphicsDevice device;

        //piirtämisen käytettävät bufferit lattialle, seinille sekä katolle
        VertexBuffer floorBuffer;
        VertexBuffer wallBuffer;
        VertexBuffer ceilingBuffer;

        Vector3[] wallPoints = new Vector3[8];
        private Random rand = new Random();
        #endregion
        #region Constructor
        public Maze(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            normalEero = content.Load<Texture2D>("erp2");
            scaryEero = content.Load<Texture2D>("erp");
            drawTexture = normalEero;
            BuildFloorBuffer();
            BuildCeilingBuffer();

            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    MazeCells[x, z] = new MazeCell();

                }

            wallPoints[0] = new Vector3(0, 1, 0);
            wallPoints[1] = new Vector3(0, 1, 1);
            wallPoints[2] = new Vector3(0, 0, 0);
            wallPoints[3] = new Vector3(0, 0, 1);
            wallPoints[4] = new Vector3(1 ,1, 0);
            wallPoints[5] = new Vector3(1 ,1, 1);
            wallPoints[6] = new Vector3(1, 0, 0);
            wallPoints[7] = new Vector3(1, 0, 1);

            GenerateMaze();
        }
        #endregion
        #region The Floor
        private void BuildFloorBuffer()
        {
            //luodaan lista verteksesitä joka annetaan sitten floorBufferille
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();

            int counter = 0;
            
            //käydään labyrintin jokainen ruutu lävitse, luoden aina uusi lattilaatta.
            for (int x = 0; x < mazeWidth; x++)
            {
                counter++;
                for (int z = 0; z < mazeHeight; z++)
                {
                    counter++;
                    foreach (VertexPositionNormalTexture vertex in FloorTile(x, z))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }

            floorBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);
            //annetaan lista joka sisältää lattian verteksit floorBufferille, jotta ne voidaan piirtää
            floorBuffer.SetData<VertexPositionNormalTexture>(vertexList.ToArray());
        }

        private List<VertexPositionNormalTexture> FloorTile(int xOffset, int zOffset)
        {
            List<VertexPositionNormalTexture> vList = new List<VertexPositionNormalTexture>();

            //jokainen lattialaatta koostuu kahdesta sivuttain makaavasta kolmiosta. VertexPositionNormalin ensimmäinen parametri on paikka,
            //toinen parametri on normaali-mappi ja kolmas parametri on teksture-mappi.
            //tekstuuri annetaan silla lailla että se vedetään loppujen lopuksi koko tiilen 
            //paint-kuva tiilestä: http://imgur.com/Bp9DUlL
            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 0, 0 + zOffset), new Vector3(0,0,0), new Vector2(0, 0)));

            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 0, 0 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 0)));

            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 0, 1 + zOffset), new Vector3(0, 0, 0), new Vector2(0, 1)));

            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 0, 0 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 0)));

            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 0, 1 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 1)));

            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 0, 1 + zOffset), new Vector3(0, 0, 0), new Vector2(0, 1)));

            //SUPERTÄRKEE HUOMIO!!!!! verteksit tulee määritellä myötäpäiväisessä järkestyksessä siihen nähden, miltä suunnalta niitä tullaan katsomaan
            //Lähes kaikki 3d-kirjastot optimoivat grafiikkaa siten, että verteksit piirretään ainoastaan toiselta puolen, silloin kun ne nähdään. Jos olet esim.
            //jossain pelissä pudonnut pelimaailman lävitse ja yhtäkkiä maailma muuttuu näkymättömäksi, se johtuu nimenomaan tästä optimointikikasta.
            //Esimerkkivideo tapahtumasta: http://www.youtube.com/watch?v=QOOXRD1LanI
            return vList;
        }

        private void BuildCeilingBuffer()
        {
            //katon luominen on täsmälleen identtinen lattian luomiseen, paitsi että katon verteksien z-arvo on 1 yksikköä suurempi kuin lattian
            //(katto on siis korkeammalla kuin lattia, kuten arvata saattaa.)
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();

            int counter = 0;

            for (int x = 0; x < mazeWidth; x++)
            {
                counter++;
                for (int z = 0; z < mazeHeight; z++)
                {
                    counter++;
                    foreach (VertexPositionNormalTexture vertex in CeilingTile(x, z))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }

            ceilingBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexList.Count, BufferUsage.WriteOnly);

            ceilingBuffer.SetData<VertexPositionNormalTexture>(vertexList.ToArray());
        }

        private List<VertexPositionNormalTexture> CeilingTile(int xOffset, int zOffset)
        {
            List<VertexPositionNormalTexture> vList = new List<VertexPositionNormalTexture>();

            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 1, 1 + zOffset), new Vector3(0,0,0), new Vector2(0, 1)));
            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 1, 1 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 1)));
            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 1, 0 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 0)));
            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 1, 1 + zOffset), new Vector3(0, 0, 0), new Vector2(0, 1)));
            vList.Add(new VertexPositionNormalTexture(new Vector3(1 + xOffset, 1, 0 + zOffset), new Vector3(0, 0, 0), new Vector2(1, 0)));
            vList.Add(new VertexPositionNormalTexture(new Vector3(0 + xOffset, 1, 0 + zOffset), new Vector3(0,0,0), new Vector2(0, 0)));

            return vList;
        }
        #endregion
        #region Walls
        private void BuildWallBuffer()
        {
            //rakennetaan labyrintin soluille piirrettävät seinät
            //homma sujuu aikalailla samalla lailla kuin lattiat ja katto
            List<VertexPositionNormalTexture> wallVertexList = new List<VertexPositionNormalTexture>();

            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    foreach (VertexPositionNormalTexture vertex in BuildMazeWall(x, z))
                    {
                        wallVertexList.Add(vertex);
                    }
                }
            }

            wallBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, wallVertexList.Count, BufferUsage.WriteOnly);

            wallBuffer.SetData<VertexPositionNormalTexture>(wallVertexList.ToArray());
        }

        private List<VertexPositionNormalTexture> BuildMazeWall(int x, int z)
        {

            List<VertexPositionNormalTexture> triangles = new List<VertexPositionNormalTexture>();
            //ottaa parametrinä solun koordinaatit
            //luodaan verteksit solun olemassaoleville seinille
            //ks. paint kuva:http://i.imgur.com/9IeADIY.png
            if (MazeCells[x, z].Walls[0])
            {
                triangles.Add(CalcPoint(0, x, z, 0, 0));
                triangles.Add(CalcPoint(4, x, z, 1, 0));
                triangles.Add(CalcPoint(2, x, z, 0, 1));
                triangles.Add(CalcPoint(4, x, z, 1, 0));
                triangles.Add(CalcPoint(6, x, z, 1, 1));
                triangles.Add(CalcPoint(2, x, z, 0, 1));
            }

            if (MazeCells[x, z].Walls[1])
            {
                 triangles.Add(CalcPoint(4, x, z, 0, 0 ));
                triangles.Add(CalcPoint(5, x, z,1, 0 ));
                triangles.Add(CalcPoint(6, x, z, 0,1));
                triangles.Add(CalcPoint(5, x, z, 1,0));
                triangles.Add(CalcPoint(7, x, z, 1,1));
                triangles.Add(CalcPoint(6, x, z, 0,1));
            }

            if (MazeCells[x, z].Walls[2])
            {
                triangles.Add(CalcPoint(5, x, z, 0,0));
                triangles.Add(CalcPoint(1, x, z, 1,0));
                triangles.Add(CalcPoint(7, x, z, 0,1));
                triangles.Add(CalcPoint(1, x, z, 1,0));
                triangles.Add(CalcPoint(3, x, z, 1,1));
                triangles.Add(CalcPoint(7, x, z,0,1 ));
            }

            if (MazeCells[x, z].Walls[3])
            {
                triangles.Add(CalcPoint(1, x, z,0,0 ));
                triangles.Add(CalcPoint(0, x, z,1,0 ));
                triangles.Add(CalcPoint(3, x, z,0,1 ));
                triangles.Add(CalcPoint(0, x, z, 1,0));
                triangles.Add(CalcPoint(2, x, z, 1,1));
                triangles.Add(CalcPoint(3, x, z, 0,1));
            }

            return triangles;
        }

        private VertexPositionNormalTexture CalcPoint(int wallPoint, int xOffset, int zOffset, int textureUOffset, int textureVOffset)
        {
            return new VertexPositionNormalTexture(wallPoints[wallPoint] + new Vector3(xOffset, 0, zOffset), new Vector3(0, 0, 0), new Vector2(textureUOffset, textureVOffset));
        }

        //tarkistetaan törmääkö pelaaja seinään
        private BoundingBox BuildBoundingBox(int x, int z, int point1, int point2)
        {
            BoundingBox thisBox = new BoundingBox(wallPoints[point1], wallPoints[point2]);

            thisBox.Min.X += x;
            thisBox.Min.Z += z;
            thisBox.Max.X += x;
            thisBox.Max.Z += z;

            thisBox.Min.X -= 0.1f;
            thisBox.Min.Z -= 0.1f;
            thisBox.Max.X += 0.1f;
            thisBox.Max.Z += 0.1f;

            return thisBox;
        }

        public List<BoundingBox> GetBoundsForCell(int x, int z)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();

            if (MazeCells[x, z].Walls[0])
                boxes.Add(BuildBoundingBox(x, z, 2, 4));
            if (MazeCells[x, z].Walls[1])
                boxes.Add(BuildBoundingBox(x, z, 6, 5));
            if (MazeCells[x, z].Walls[2])
                boxes.Add(BuildBoundingBox(x, z, 3, 5));
            if (MazeCells[x, z].Walls[3])
                boxes.Add(BuildBoundingBox(x, z, 2, 1));

            return boxes;
        }

        private void RemoveRandomWalls()
        {

            for (int i = 0; i < 1000; i++)
            {
                int randomHeight = rand.Next(0, mazeHeight);
                int randomWidth = rand.Next(0, mazeWidth);

                MazeCells[randomHeight, randomWidth].Visited = false;

                EvaluateCell(new Vector2(randomHeight, randomWidth));
            }
        }

        #endregion
        #region Maze Generation
        public void GenerateMaze()
        {
            //Generoidaan satunnainen labyrintti hyödyntämällä DFS-algoritmiä.
            //ks. http://en.wikipedia.org/wiki/Maze_generation_algorithm#Depth-first_search
            //alussa kaikkien labyrintin solujen seinät ovat pystyssä, eikä missään solussa ole vierailtu
            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    MazeCells[x, z].Walls[0] = true;
                    MazeCells[x, z].Walls[1] = true;
                    MazeCells[x, z].Walls[2] = true;
                    MazeCells[x, z].Walls[3] = true;
                    MazeCells[x, z].Visited = false;
                }
            //aloiteaan solusta 0,0 labyrintin läpikäynti
            MazeCells[0,0].Visited = true;
            EvaluateCell(new Vector2(0, 0));
            //Koska DFS antaa meille "klassisen" labyrintin joissa on paljon tiukkoja mutkia ja umpikujia, poistetaan seiniä satunnaisesti
            RemoveRandomWalls();
            BuildWallBuffer();
        }

        private void EvaluateCell(Vector2 cell)
        {
            //lista käsiteltävissä olevan solun naapureista.
            // 0 = solun yläpuolella oleva naapuri
            // 1= solun oikealla puolella oleva naapuri
            // 2 = solun alapuolella oleva naapuri
            // 3 = solun vasemmalla puolella oleva naapuri
            List<int> neighborCells = new List<int>();
            neighborCells.Add(0);
            neighborCells.Add(1);
            neighborCells.Add(2);
            neighborCells.Add(3);

            while (neighborCells.Count > 0)
            {
                //arvotaan käsiteltävän solun satunnainen naapuri, poistetaan se käsiteltävissä ja otetaan talteen selectedNieghhbot-muuttujaan
                int pick = rand.Next(0, neighborCells.Count);
                int selectedNeighbor = neighborCells[pick];
                neighborCells.RemoveAt(pick);

                Vector2 neighbor = cell;

                switch (selectedNeighbor)
                {
                    case 0: neighbor += new Vector2(0, -1);
                        break;
                    case 1: neighbor += new Vector2(1, 0);
                        break;
                    case 2: neighbor += new Vector2(0, 1);
                        break;
                    case 3: neighbor += new Vector2(-1, 0);
                        break;
                }

                if (
                    (neighbor.X >= 0) &&
                    (neighbor.X < mazeWidth) &&
                    (neighbor.Y >= 0) &&
                    (neighbor.Y < mazeHeight)
                    )
                {
                    //asetetaan käsiteltävissä olevan solun randomilla valittu seinä sekä naapurin sitä vastapäinen seinä
                    //olemattomaksi. Tämä onnistuu kätevästi modulon avulla.
                    //jos esim. valittu naapuri on käsiteltäbän solun oikella puolen, on se solu 1.
                    //aluksi poistetaan käsiteltävän solun seinä nro 1.
                    //sitten poistetaan naapurin seinä nro. 3
                    if (!MazeCells[(int)neighbor.X, (int)neighbor.Y].Visited)
                    {
                        MazeCells[(int)neighbor.X, (int)neighbor.Y].Visited = true;
                        MazeCells[(int)cell.X, (int)cell.Y].Walls[selectedNeighbor] = false;
                        MazeCells[(int)neighbor.X, (int)neighbor.Y].Walls[(selectedNeighbor + 2) % 4 ] = false;
                        EvaluateCell(neighbor);
                    }
                }
            }
        }
        #endregion
        #region Draw

        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.TextureEnabled = true;
            effect.Texture = drawTexture;

            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.EnableDefaultLighting();
            effect.PreferPerPixelLighting = true;

            //effect.AmbientLightColor = new Vector3(0.1f, 0f, 0f);
            effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.FogEnabled = true;
            effect.FogColor = fogColor;
            effect.FogStart = 0f;
            effect.FogEnd = 2.5f;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(floorBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);

                device.SetVertexBuffer(wallBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, wallBuffer.VertexCount / 3);

                device.SetVertexBuffer(ceilingBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, ceilingBuffer.VertexCount / 3);
            }
        }
        #endregion
    }
}