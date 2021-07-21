using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Sides {
    public const int NumSidesSquare = 4; // it's hip to be square
    public const int NumSidesCube = 6; // it's square to be cube
	public const int Undefined = -1;
	public const int U = 0;
	public const int R = 1;
    public const int D = 2;
    public const int L = 3;
    public const int F = 4;
    public const int B = 5;
    
    static public string GetName(int side) {
        switch (side) {
            case Sides.L: return "Left";
            case Sides.R: return "Right";
            case Sides.D: return "Down";
            case Sides.U: return "Up";
            case Sides.B: return "Back";
            case Sides.F: return "Front";
            default: throw new UnityException ("Whoa, " + side + " is not a valid side. Try 0 through 5.");
        }
    }

	static public int GetOpposite(int side) {
		switch (side) {
			case Sides.L: return Sides.R;
			case Sides.R: return Sides.L;
			case Sides.D: return Sides.U;
			case Sides.U: return Sides.D;
			default: throw new UnityException ("Whoa, " + side + " is not a valid side. Try 0 through 3.");
		}
	}
	static public int GetHorzFlipped(int side) {
		switch (side) {
			case L: return R;
			case R: return L;
			default: return side; // this side isn't affected by a flip.
		}
	}
	static public int GetVertFlipped(int side) {
		switch (side) {
			case U: return D;
			case D: return U;
			default: return side; // this side isn't affected by a flip.
		}
	}
}
