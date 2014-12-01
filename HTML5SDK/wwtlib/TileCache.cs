using System;
using System.Collections.Generic;
using System.Linq;

namespace wwtlib
{
    class TileCache
    {
        private static Dictionary<String, Tile> queue = new Dictionary<String, Tile>();
        static Dictionary<String, Tile> tiles = new Dictionary<string, Tile>();
        //internal static void AddTileToQueue(Tile tile)
        //{

        //    queue[tile.Key] = tile;
        //}

        public static Tile GetTile(int level, int x, int y, Imageset dataset, Tile parent)
        {

            Tile retTile = null;
            string tileKey = Imageset.GetTileKey(dataset, level, x, y);
           // try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    retTile = Imageset.GetNewTile(dataset, level, x, y, parent);
                    tiles[tileKey] = retTile;

                }
                else
                {
                    retTile = tiles[tileKey];
                }
            }
           // catch
            {
                int p = 0;
            }

            return retTile;

        }

        public static Tile GetCachedTile(int level, int x, int y, Imageset dataset, Tile parent)
        {
            if (level < dataset.BaseLevel)
            {
                return null;
            }

            Tile retTile = null;
            string tileKey = Imageset.GetTileKey(dataset, level, x, y);
            try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    return null;
                }
                else
                {
                    retTile = tiles[tileKey];
                }
            }
            catch
            {
            }

            return retTile;
        }

        public static int GetReadyToRenderTileCount()
        {
            List<Tile> notReadyCullList = new List<Tile>();
            List<Tile> readyCullList = new List<Tile>();

            try
            {
                try
                {
                    foreach (string key in tiles.Keys)
                    {
                        Tile tile = tiles[key];

                        if (tile.RenderedGeneration < (Tile.CurrentRenderGeneration - 10) && !(tile.RequestPending || tile.Downloading))
                        {
                            if (tile.ReadyToRender)
                            {
                                readyCullList.Add(tile);
                            }
                            else
                            {
                                notReadyCullList.Add(tile);
                            }
                        }
                    }
                }
                catch
                {
                    
                }
                return readyCullList.Count;
            }
            catch
            {
                return -1;
            }
        }

        const int MaxDownloadCount = 8;

        public static int openThreads = 8;

        public static void ProcessQueue(RenderContext renderContext)
        {

            while(queue.Count > 0 && openThreads > 0)
            {
               

                double minDistance = 100000.0f;
                bool overlayTile = false;
                string maxKey = null;
                int level = 1000;

                foreach (String key in queue.Keys)
                {
                    Tile t = queue[key];
                    if (!t.RequestPending && t.InViewFrustum)
                    {

                        Vector3d vectTemp = Vector3d.MakeCopy(t.SphereCenter);

                        vectTemp.TransformByMatrics(renderContext.World);

                        if (renderContext.Space)
                        {
                            vectTemp.Subtract(Vector3d.Create(0.0f, 0.0f, -1.0f));
                        }
                        else
                        {
                            vectTemp.Subtract(renderContext.CameraPosition);
                        }

                        double distTemp = Math.Max(0, vectTemp.Length() - t.SphereRadius);



                        //if (t.Level < (level-1) || (distTemp < minDistance && t.Level == level))
                        bool thisIsOverlay = (t.Dataset.Projection == ProjectionType.Tangent) || (t.Dataset.Projection == ProjectionType.SkyImage);
                        if (distTemp < minDistance && (!overlayTile || thisIsOverlay))
                        {
                            minDistance = distTemp;
                            maxKey = t.Key;
                            level = t.Level;
                            overlayTile = thisIsOverlay;
                        }
                    }

                }
                if (maxKey != null)
                {
                    Tile workTile = (Tile)queue[maxKey];
                    workTile.RequestPending = true;
                    openThreads--;
                    if (openThreads < 0)
                    {
                        openThreads = 0;
                    }
                    workTile.RequestImage();
                    if (workTile.Dataset.ElevationModel )// && workTile.Dataset.Projection == ProjectionType.Toast)
                    {
                        workTile.RequestDem();
                    }
                }
                else
                {
                    return;
                }
            }
        }

       
        public static bool AddTileToQueue(Tile tile)
        {
          
            int hitValue;

            hitValue = 256;


            if (!tile.Downloading && !tile.ReadyToRender)
            {

                if (queue.ContainsKey(tile.Key))
                {
                    (queue[tile.Key]).RequestHits += hitValue;
                }
                else
                {
                    tile.RequestHits = hitValue;
                    queue[tile.Key] = tile;
                }
            }

            return true;
        }

        public static void RemoveFromQueue(string key, bool complete)
        {
            if (complete)
            {
                Tile workTile = (Tile)queue[key];
                if (workTile != null)
                {
                    workTile.RequestPending = false;
                    queue.Remove(workTile.Key);
                }
                openThreads++;
            }
            queue.Remove(key);
        }

        public static void ClearCache()
        {

            tiles.Clear();

        }

        public static void PurgeQueue()
        {

            queue.Clear();

        }

        public static int readyToRenderCount = 0;
        public static int maxTileCacheSize = 800;
        public static int maxReadyToRenderSize = 200;
        public static int AccessID = 0;

        static int maxTotalToPurge = 0;
        public static void PurgeLRU()
        {
            if (tiles.Count < maxReadyToRenderSize)
            {
                return;
            }

            List<Tile> notReadyCullList = new List<Tile>();
            List<Tile> readyCullList = new List<Tile>();

            try
            {
                try
                {
                    foreach (string key in tiles.Keys)
                    {
                        Tile tile = tiles[key];

                        if (tile.RenderedGeneration < (Tile.CurrentRenderGeneration - 10) && !(tile.RequestPending || tile.Downloading))
                        {
                            if (tile.ReadyToRender)
                            {
                                readyCullList.Add(tile);
                            }
                            else
                            {
                                notReadyCullList.Add(tile);
                            }
                        }
                    }
                }
                catch
                {
                }
                readyToRenderCount = readyCullList.Count;

                if (readyCullList.Count > maxReadyToRenderSize)
                {
                    readyCullList.Sort(delegate(Tile t1, Tile t2)
                    {
                        return t2.AccessCount < t1.AccessCount ? 1 : (t2.AccessCount == t1.AccessCount ? 0 : -1);
                    }
                    );
                    int totalToPurge = readyCullList.Count - maxReadyToRenderSize;

                    foreach (Tile tile in readyCullList)
                    {
                        if (totalToPurge < 1)
                        {
                            break;
                        }
                        tile.CleanUp(false);

                        //tile.CleanUp(true);
                        //tiles.Remove(tile.Key);
                        totalToPurge--;
                    }
                }

                if (tiles.Count < maxTileCacheSize)
                {
                    return;
                }

                if (notReadyCullList.Count > maxTileCacheSize)
                {
                    notReadyCullList.Sort(delegate(Tile t1, Tile t2)
                    {
                        return t2.AccessCount < t1.AccessCount ? 1 : (t2.AccessCount == t1.AccessCount ? 0 : -1);
                    }
                    );

                    int totalToPurge = notReadyCullList.Count - maxTileCacheSize;
                    if (totalToPurge > 20)
                    {
                        totalToPurge = 20;
                    }
                    foreach (Tile tile in notReadyCullList)
                    {
                        if (totalToPurge < 1)
                        {
                            break;
                        }
                        tile.CleanUp(true);
                        tiles.Remove(tile.Key);

                        totalToPurge--;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                
            }

            return;
        }


        // Age things in queue. If they are not visible they will go away in time
        public static void DecimateQueue()
        {
            List<Tile> list = new List<Tile>();

            foreach (string key in queue.Keys)
            {
                Tile t = queue[key];
                if (!t.RequestPending)
                {
                    t.RequestHits = t.RequestHits / 2;
                    try
                    {
                        if (t.RequestHits < 2)// && !t.DirectLoad)
                        {
                            list.Add(t);
                        }
                        else if (!t.InViewFrustum)
                        {
                            list.Add(t);
                        }
                    }
                    catch
                    {
                    }
                }

            }
            foreach (Tile t in list)
            {
                queue.Remove(t.Key);
            }
           


        }
    }
}
