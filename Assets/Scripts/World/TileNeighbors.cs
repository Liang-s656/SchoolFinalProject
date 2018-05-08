public class TileNeighbors {
    public Tile top, right, bottom, left;
    public Tile[] ToArray(){
        return new Tile[]{top, right, bottom, left};
    }
}
