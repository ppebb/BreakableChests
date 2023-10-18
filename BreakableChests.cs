using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BreakableChests {
    public class BreakableChests : Mod {
        public override void Load() {
            On_Chest.CanDestroyChest += CanDestroyChest;
            On_Chest.DestroyChest += DestroyChest;
        }

        private bool CanDestroyChest(On_Chest.orig_CanDestroyChest orig, int X, int Y) {
            for (int i = 0; i < 8000; i++) {
                Chest chest = Main.chest[i];

                if (chest == null || chest.x != X || chest.y != Y)
                    continue;

                if (ChestIsOpenByAnotherPlayerOrLocked(i, X, Y))
                    return false;
            }

            return true;
        }

        private bool DestroyChest(On_Chest.orig_DestroyChest orig, int X, int Y) {
            for (int i = 0; i < 8000; i++) {
                Chest chest = Main.chest[i];

                if (chest == null || chest.x != X || chest.y != Y)
                    continue;

                if (ChestIsOpenByAnotherPlayerOrLocked(i, X, Y))
                    return false;

                foreach (Item item in chest.item) {
                    if (!item.IsAir) {
                        int index = Item.NewItem(new EntitySource_TileBreak(X, Y, null), new Vector2(X * 16, Y * 16), item);
                        Item worldItem = Main.item[index];
                        worldItem.velocity = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-3, 0));
                    }
                }

                Chest.DestroyChestDirect(X, Y, i);
                return true;
            }

            return true;
        }

        private bool ChestIsOpenByAnotherPlayerOrLocked(int i, int x, int y) {
            for (int j = 0; j < Main.maxPlayers; j++) {
                Player player = Main.player[j];
                if (i != Main.myPlayer && player.chest == i && player.active)
                    return true;
            }

            if (Chest.IsLocked(x, y))
                return true;

            return false; // This should NOT be broken!!!
        }
    }
}
