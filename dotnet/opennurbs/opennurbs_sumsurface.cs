using System;

namespace Rhino.Geometry
{
  public class SumSurface : Surface
  {
    /// <summary>
    /// Create a SumSurface by extruding a curve (CurveA) along a path (CurveB)
    /// </summary>
    /// <param name="curveA"></param>
    /// <param name="curveB"></param>
    /// <returns>SumSurface on success, null on failure</returns>
    public static SumSurface Create(Curve curveA, Curve curveB)
    {
      IntPtr pConstCurveA = curveA.ConstPointer();
      IntPtr pConstCurveB = curveB.ConstPointer();
      IntPtr pSumSurface = UnsafeNativeMethods.ON_SumSurface_Create(pConstCurveA, pConstCurveB);
      if (IntPtr.Zero == pSumSurface)
        return null;
      return new SumSurface(pSumSurface, null);
    }

    internal SumSurface(IntPtr ptr, object parent)
      : base(ptr, parent)
    { }

    internal override GeometryBase DuplicateShallowHelper()
    {
      return new SumSurface(IntPtr.Zero, null);
    }
  }
}
