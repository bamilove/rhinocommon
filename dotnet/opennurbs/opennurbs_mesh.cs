using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using System.Runtime.InteropServices;
using System.Diagnostics;

using Rhino.Collections;
using Rhino;

//public class ON_MeshParameters { }
//public class ON_MeshCurvatureStats { }
//public class ON_MeshTopologyVertex { }
//public class ON_MeshTopologyEdge { }
//public class ON_MeshTopologyFace { }
//public struct ON_MeshFace { }
//public struct ON_MeshPart { }
//public class ON_MeshTopology { }
//public class ON_MeshNgon { }
//public class ON_MeshNgonList { }
//public class ON_MeshPartition { }
//public class ON_MappingTag { }
//public class ON_TextureCoordinates { }
//public class ON_MeshVertexRef : ON_Geometry { }
//public class ON_MeshEdgeRef : ON_Geometry { }
//public class ON_MeshFaceRef : ON_Geometry { }
namespace Rhino.Geometry
{
  /// <summary>
  /// Represents mesher settings for Brep->Mesh conversions.
  /// </summary>
  public struct MeshParameters
  {
    #region fields
    internal bool m_jagged;
    internal bool m_simple;
    internal bool m_refine;
    internal bool m_curvature;

    internal int m_min_count;
    internal int m_max_count;
    internal int m_facetype; //not exposed yet.

    internal double m_grid_amp;           //0
    internal double m_tolerance;          //1
    internal double m_grid_angle;         //2
    internal double m_grid_aspect;        //3
    internal double m_refine_angle;       //4
    internal double m_min_tolerance;      //5
    internal double m_max_edge_length;    //6
    internal double m_min_edge_length;    //7
    internal double m_relative_tolerance; //8
    #endregion

    internal static MeshParameters FromPointer(IntPtr pConstMeshParameters)
    {
      bool[] bvals = new bool[4];
      int[] ivals = new int[3];
      double[] dvals = new double[9];
      MeshParameters rc = new MeshParameters();
      if (!UnsafeNativeMethods.ON_MeshParameters_Copy(pConstMeshParameters, bvals, ivals, dvals))
        return rc;
      rc.m_jagged = bvals[0];
      rc.m_simple = bvals[1];
      rc.m_refine = bvals[2];
      rc.m_curvature = bvals[3];

      rc.m_min_count = ivals[0];
      rc.m_max_count = ivals[1];
      rc.m_facetype = ivals[2];

      rc.m_grid_amp = dvals[0];
      rc.m_tolerance = dvals[1];
      rc.m_grid_angle = dvals[2];
      rc.m_grid_aspect = dvals[3];
      rc.m_refine_angle = dvals[4];
      rc.m_min_tolerance = dvals[5];
      rc.m_max_edge_length = dvals[6];
      rc.m_min_edge_length = dvals[7];
      rc.m_relative_tolerance = dvals[8];
      return rc;
    }

    #region constants
    /// <summary>
    /// Gets minimal meshing parameters. 
    /// Use this rather than the default constructor as it 
    /// initialized important fields properly.
    /// </summary>
    public static MeshParameters Minimal
    {
      get
      {
        MeshParameters mp = new MeshParameters();

        mp.m_jagged = true;
        mp.m_refine = false;
        mp.m_simple = false;
        mp.m_curvature = false;

        mp.m_facetype = 0;
        mp.m_min_count = 16;
        mp.m_max_count = 0;

        mp.m_grid_amp = 1.0;
        mp.m_grid_angle = 0.0;
        mp.m_grid_aspect = 6.0;

        mp.m_tolerance = 0.0;
        mp.m_min_tolerance = 0.0;

        mp.m_min_edge_length = 0.0001;
        mp.m_max_edge_length = 0.0;

        mp.m_refine_angle = 0.0;
        mp.m_relative_tolerance = 0.0;

        return mp;
      }
    }

    /// <summary>
    /// Gets default meshing parameters. 
    /// Use this rather than the default constructor as it 
    /// initialized important fields properly.
    /// </summary>
    public static MeshParameters Default
    {
      get
      {
        MeshParameters mp = new MeshParameters();

        mp.m_jagged = false;
        mp.m_refine = true;
        mp.m_simple = false;
        mp.m_curvature = false;

        mp.m_facetype = 0;
        mp.m_min_count = 0;
        mp.m_max_count = 0;

        mp.m_grid_amp = 1.0;
        mp.m_grid_angle = (20.0 * Math.PI) / 180.0;
        mp.m_grid_aspect = 6.0;

        mp.m_tolerance = 0.0;
        mp.m_min_tolerance = 0.0;

        mp.m_min_edge_length = 0.0001;
        mp.m_max_edge_length = 0.0;

        mp.m_refine_angle = (20.0 * Math.PI) / 180.0;
        mp.m_relative_tolerance = 0.0;

        return mp;
      }
    }
    /// <summary>
    /// Gets meshing parameters for coarse meshing. 
    /// This corresponds with the "Jagged and Faster" default in Rhino.
    /// </summary>
    public static MeshParameters Coarse
    {
      get
      {
        MeshParameters mp = Default;

        mp.m_grid_amp = 0.0;
        mp.m_grid_angle = 0.0;
        mp.m_grid_aspect = 0.0;
        mp.m_refine_angle = 0.0;

        mp.m_relative_tolerance = 0.65;
        mp.m_min_count = 16;
        mp.m_min_edge_length = 0.0001;
        mp.m_simple = true;

        return mp;
      }
    }
    /// <summary>
    /// Gets meshing parameters for smooth meshing. 
    /// This corresponds with the "Smooth and Slower" default in Rhino.
    /// </summary>
    public static MeshParameters Smooth
    {
      get
      {
        MeshParameters mp = Default;

        mp.m_grid_amp = 0.0;
        mp.m_grid_angle = 0.0;
        mp.m_grid_aspect = 0.0;
        mp.m_refine_angle = 0.0;

        mp.m_relative_tolerance = 0.8;
        mp.m_min_count = 16;
        mp.m_min_edge_length = 0.0001;
        mp.m_simple = true;
        mp.m_refine_angle = (20.0 * Math.PI) / 180.0;

        return mp;
      }
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets whether or not the mesh is allowed to have jagged seams. 
    /// When this flag is set to True, meshes on either side of a Brep Edge will not match up.
    /// </summary>
    public bool JaggedSeams
    {
      get { return m_jagged; }
      set { m_jagged = value; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether or not the sampling grid can be refined 
    /// when certain tolerances are not met.
    /// </summary>
    public bool RefineGrid
    {
      get { return m_refine; }
      set { m_refine = value; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether or not planar areas are allowed 
    /// to be meshed in a simplified manner.
    /// </summary>
    public bool SimplePlanes
    {
      get { return m_simple; }
      set { m_simple = value; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether or not surface curvature 
    /// data will be embedded in the mesh.
    /// </summary>
    public bool ComputeCurvature
    {
      get { return m_curvature; }
      set { m_curvature = value; }
    }

    /// <summary>
    /// Gets or sets the minimum number of grid quads in the initial sampling grid.
    /// </summary>
    public int GridMinCount
    {
      get { return m_min_count; }
      set { m_min_count = value; }
    }
    /// <summary>
    /// Gets or sets the maximum number of grid quads in the initial sampling grid.
    /// </summary>
    public int GridMaxCount
    {
      get { return m_max_count; }
      set { m_max_count = value; }
    }

    /// <summary>
    /// Gets or sets the maximum allowed angle difference (in radians) 
    /// for a single sampling quad. The angle pertains to the surface normals.
    /// </summary>
    public double GridAngle
    {
      get { return m_grid_angle; }
      set { m_grid_angle = value; }
    }
    /// <summary>
    /// Gets or sets the maximum allowed aspect ratio of sampling quads.
    /// </summary>
    public double GridAspectRatio
    {
      get { return m_grid_aspect; }
      set { m_grid_aspect = value; }
    }
    /// <summary>
    /// Gets or sets the grid amplification factor. 
    /// Values lower than 1.0 will decrease the number of initial quads, 
    /// values higher than 1.0 will increase the number of initial quads.
    /// </summary>
    public double GridAmplification
    {
      get { return m_grid_amp; }
      set { m_grid_amp = value; }
    }

    /// <summary>
    /// Gets or sets the maximum allowed edge deviation. 
    /// This tolerance is measured between the center of the mesh edge and the surface.
    /// </summary>
    public double Tolerance
    {
      get { return m_tolerance; }
      set { m_tolerance = value; }
    }
    /// <summary>
    /// Gets or sets the minimum tolerance.
    /// </summary>
    public double MinimumTolerance
    {
      //David: I don't understand what this does... Someone please finish the xml comments.
      get { return m_min_tolerance; }
      set { m_min_tolerance = value; }
    }
    /// <summary>
    /// Gets or sets the relative tolerance.
    /// </summary>
    public double RelativeTolerance
    {
      //David: I don't understand what this does... Someone please finish the xml comments.
      get { return m_relative_tolerance; }
      set { m_relative_tolerance = value; }
    }

    /// <summary>
    /// Gets or sets the minimum allowed mesh edge length.
    /// </summary>
    public double MinimumEdgeLength
    {
      get { return m_min_edge_length; }
      set { m_min_edge_length = value; }
    }
    /// <summary>
    /// Gets or sets the maximum allowed mesh edge length.
    /// </summary>
    public double MaximumEdgeLength
    {
      get { return m_max_edge_length; }
      set { m_max_edge_length = value; }
    }
    #endregion
  }

  public class Mesh : GeometryBase
  {
    #region static mesh creation
    /// <summary>
    /// Create a planar mesh grid.
    /// </summary>
    /// <param name="plane">Plane of mesh.</param>
    /// <param name="xInterval">Interval describing size and extends of mesh along plane x-direction.</param>
    /// <param name="yInterval">Interval describing size and extends of mesh along plane y-direction.</param>
    /// <param name="xCount">Number of faces in x-direction.</param>
    /// <param name="yCount">Number of faces in y-direction.</param>
    /// <exception cref="ArgumentNullException">Thrown when plane is a null reference.</exception>
    /// <exception cref="ArgumentNullException">Thrown when xInterval is a null reference.</exception>
    /// <exception cref="ArgumentNullException">Thrown when yInterval is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when xCount is less than or equal to zero.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when yCount is less than or equal to zero.</exception>
    public static Mesh CreateFromPlane(Plane plane, Interval xInterval, Interval yInterval, int xCount, int yCount)
    {
      if (!plane.IsValid) { throw new ArgumentException("plane is invalid"); }
      if (!xInterval.IsValid) { throw new ArgumentException("xInterval is invalid"); }
      if (!yInterval.IsValid) { throw new ArgumentException("yInterval is invalid"); }
      if (xCount <= 0) { throw new ArgumentOutOfRangeException("xCount"); }
      if (yCount <= 0) { throw new ArgumentOutOfRangeException("yCount"); }

      IntPtr ptr = UnsafeNativeMethods.ON_Mesh_CreateMeshPlane(ref plane, xInterval, yInterval, xCount, yCount);

      if (IntPtr.Zero == ptr)
        return null;
      return new Mesh(ptr, null, null);
    }

    /// <summary>
    /// Create a mesh sphere.
    /// </summary>
    /// <param name="sphere">Base sphere for mesh.</param>
    /// <param name="xCount">Number of faces in the around direction.</param>
    /// <param name="yCount">Number of faces in the top-to-bottom direction.</param>
    /// <exception cref="ArgumentNullException">Thrown when sphere is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when xCount is less than or equal to two.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when yCount is less than or equal to two.</exception>
    public static Mesh CreateFromSphere(Sphere sphere, int xCount, int yCount)
    {
      if (!sphere.IsValid) { throw new ArgumentException("sphere is invalid"); }
      if (xCount < 2) { throw new ArgumentOutOfRangeException("xCount"); }
      if (yCount < 2) { throw new ArgumentOutOfRangeException("yCount"); }

      IntPtr ptr = UnsafeNativeMethods.ON_Mesh_CreateMeshSphere(ref sphere.m_plane, sphere.m_radius, xCount, yCount);

      if (IntPtr.Zero == ptr)
        return null;
      return new Mesh(ptr, null, null);
    }

#if USING_V5_SDK
    /// <summary>
    /// Attempt to create a mesh from a closed planar curve
    /// </summary>
    /// <param name="boundary">must be a closed planar curve</param>
    /// <returns>
    /// new Mesh on success
    /// null on failure
    /// </returns>
    static public Mesh CreateFromPlanarBoundary(Curve boundary)
    {
      if (null == boundary)
        return null;
      IntPtr pCurve = boundary.ConstPointer();
      IntPtr pMesh = UnsafeNativeMethods.ON_Mesh_FromPlanarCurve(pCurve);
      if (IntPtr.Zero == pMesh)
        return null;
      return new Mesh(pMesh, null, null);
    }
#endif

    /// <summary>
    /// Create a mesh from a Brep.
    /// </summary>
    /// <param name="brep">Brep to approximate.</param>
    /// <returns>An array of meshes.</returns>
    public static Mesh[] CreateFromBrep(Brep brep)
    {
      IntPtr pConstBrep = brep.ConstPointer();
      Runtime.InteropWrappers.SimpleArrayMeshPointer meshes = new Rhino.Runtime.InteropWrappers.SimpleArrayMeshPointer();
      IntPtr pMeshes = meshes.NonConstPointer();
      int count = UnsafeNativeMethods.ON_Brep_CreateMesh(pConstBrep, pMeshes);
      Mesh[] rc = null;
      if (count > 0)
        rc = meshes.ToNonConstArray();
      meshes.Dispose();
      return rc;
    }
    /// <summary>
    /// Create a mesh from a Brep.
    /// </summary>
    /// <param name="brep">Brep to approximate.</param>
    /// <param name="meshParameters">Parameters to use during meshing.</param>
    /// <returns>An array of meshes.</returns>
    public static Mesh[] CreateFromBrep(Brep brep, MeshParameters meshParameters)
    {
      IntPtr pConstBrep = brep.ConstPointer();
      Runtime.InteropWrappers.SimpleArrayMeshPointer meshes = new Rhino.Runtime.InteropWrappers.SimpleArrayMeshPointer();
      IntPtr pMeshes = meshes.NonConstPointer();
      //David: yeah yeah yeah. I know this is hideous.
      int count = UnsafeNativeMethods.ON_Brep_CreateMesh2(pConstBrep, pMeshes,
                                                          meshParameters.m_simple,
                                                          meshParameters.m_refine,
                                                          meshParameters.m_jagged,
                                                          meshParameters.m_curvature,
                                                          meshParameters.m_min_count,
                                                          meshParameters.m_max_count,
                                                          meshParameters.m_facetype,
                                                          meshParameters.m_tolerance,
                                                          meshParameters.m_min_tolerance,
                                                          meshParameters.m_relative_tolerance,
                                                          meshParameters.m_grid_amp,
                                                          meshParameters.m_grid_angle,
                                                          meshParameters.m_grid_aspect,
                                                          meshParameters.m_refine_angle,
                                                          meshParameters.m_min_edge_length,
                                                          meshParameters.m_max_edge_length);
      Mesh[] rc = null;
      if (count > 0)
        rc = meshes.ToNonConstArray();
      meshes.Dispose();
      return rc;
    }


    //public static Mesh CreateFromSurface(Surface surface)
    //{
    //}
    //public static Mesh CreateFromBrepFace(BrepFace face)
    //{
    //}

    /// <summary>
    /// Compute the Solid Union of a set of Meshes.
    /// </summary>
    /// <param name="meshes">Meshes to union.</param>
    /// <returns>An array of Mesh results or null on failure.</returns>
    public static Mesh[] CreateBooleanUnion(IEnumerable<Mesh> meshes)
    {
      if (null == meshes)
        return null;

      Runtime.InteropWrappers.SimpleArrayMeshPointer input = new Runtime.InteropWrappers.SimpleArrayMeshPointer();
      foreach (Mesh mesh in meshes)
      {
        if (null == mesh)
          continue;
        input.Add(mesh, true);
      }
      Runtime.InteropWrappers.SimpleArrayMeshPointer output = new Runtime.InteropWrappers.SimpleArrayMeshPointer();

      IntPtr pInput = input.ConstPointer();
      IntPtr pOutput = output.NonConstPointer();

      // Fugier uses the following two tolerances in RhinoScript for all MeshBooleanUnion
      // calculations.
      const double MeshBoolIntersectionTolerance = RhinoMath.SqrtEpsilon * 10.0;
      const double MeshBoolOverlapTolerance = RhinoMath.SqrtEpsilon * 10.0;

      Mesh[] rc = null;
      if (UnsafeNativeMethods.RHC_RhinoMeshBooleanUnion(pInput, MeshBoolIntersectionTolerance, MeshBoolOverlapTolerance, pOutput))
      {
        rc = output.ToNonConstArray();
      }

      input.Dispose();
      output.Dispose();

      return rc;
    }

    const int idxIntersect = 0;
    const int idxDifference = 1;
    const int idxSplit = 2;
    static Mesh[] MeshBooleanHelper(IEnumerable<Mesh> firstSet, IEnumerable<Mesh> secondSet, int which)
    {
      if (null == firstSet || null == secondSet)
        return null;

      Runtime.InteropWrappers.SimpleArrayMeshPointer input1 = new Runtime.InteropWrappers.SimpleArrayMeshPointer();
      foreach (Mesh mesh in firstSet)
      {
        if (null == mesh)
          continue;
        input1.Add(mesh, true);
      }

      Runtime.InteropWrappers.SimpleArrayMeshPointer input2 = new Runtime.InteropWrappers.SimpleArrayMeshPointer();
      foreach (Mesh mesh in secondSet)
      {
        if (null == mesh)
          continue;
        input2.Add(mesh, true);
      }

      Runtime.InteropWrappers.SimpleArrayMeshPointer output = new Runtime.InteropWrappers.SimpleArrayMeshPointer();

      IntPtr pInput1 = input1.ConstPointer();
      IntPtr pInput2 = input2.ConstPointer();
      IntPtr pOutput = output.NonConstPointer();

      // Fugier uses the following two tolerances in RhinoScript for all MeshBoolean...
      // calculations.
      const double MeshBoolIntersectionTolerance = RhinoMath.SqrtEpsilon * 10.0;
      const double MeshBoolOverlapTolerance = RhinoMath.SqrtEpsilon * 10.0;

      Mesh[] rc = null;
      if (UnsafeNativeMethods.RHC_RhinoMeshBooleanIntDiff(pInput1, pInput2, MeshBoolIntersectionTolerance, MeshBoolOverlapTolerance, pOutput, which))
      {
        rc = output.ToNonConstArray();
      }

      input1.Dispose();
      input2.Dispose();
      output.Dispose();

      return rc;
    }

    /// <summary>
    /// Compute the Solid Difference of two sets of Meshes.
    /// </summary>
    /// <param name="firstSet">First set of Meshes (the set to subtract from).</param>
    /// <param name="secondSet">Second set of Meshes (the set to subtract).</param>
    /// <returns>An array of Mesh results or null on failure.</returns>
    public static Mesh[] CreateBooleanDifference(IEnumerable<Mesh> firstSet, IEnumerable<Mesh> secondSet)
    {
      return MeshBooleanHelper(firstSet, secondSet, idxDifference);
    }
    /// <summary>
    /// Compute the Solid Intersection of two sets of Meshes.
    /// </summary>
    /// <param name="firstSet">First set of Meshes.</param>
    /// <param name="secondSet">Second set of Meshes.</param>
    /// <returns>An array of Mesh results or null on failure.</returns>
    public static Mesh[] CreateBooleanIntersection(IEnumerable<Mesh> firstSet, IEnumerable<Mesh> secondSet)
    {
      return MeshBooleanHelper(firstSet, secondSet, idxIntersect);
    }

    public static Mesh[] CreateBooleanSplit(IEnumerable<Mesh> meshesToSplit, IEnumerable<Mesh> meshSplitters)
    {
      return MeshBooleanHelper(meshesToSplit, meshSplitters, idxSplit);
    }
    #endregion

    #region constructors
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public Mesh()
    {
      IntPtr ptr = UnsafeNativeMethods.ON_Mesh_New(IntPtr.Zero);
      ConstructNonConstObject(ptr);
      ApplyMemoryPressure();
    }

    internal override IntPtr _InternalGetConstPointer()
    {
      if (null != m_parent_face)
        return m_parent_face._GetMeshPointer();

      return base._InternalGetConstPointer();
    }

    BrepFace m_parent_face;
    internal override object _GetConstObjectParent()
    {
      if (!IsDocumentControlled)
        return null;
      if (null != m_parent_face)
        return m_parent_face;
      return base._GetConstObjectParent();
    }

    protected override void OnSwitchToNonConst()
    {
      m_parent_face = null;
      base.OnSwitchToNonConst();
    }

    internal Mesh(BrepFace parent_face)
    {
      m_parent_face = parent_face;
    }

    internal Mesh(IntPtr native_pointer, Rhino.DocObjects.RhinoObject parent_object, Rhino.DocObjects.ObjRef obj_ref)
      : base(native_pointer, parent_object, obj_ref)
    {
      if (null == parent_object)
        ApplyMemoryPressure();
    }

    public override GeometryBase Duplicate()
    {
      IntPtr ptr = ConstPointer();
      IntPtr pNewMesh = UnsafeNativeMethods.ON_Mesh_New(ptr);
      return new Mesh(pNewMesh, null, null);
    }
    /// <summary>
    /// Create an exact duplicate of this Mesh.
    /// </summary>
    public Mesh DuplicateMesh()
    {
      return Duplicate() as Mesh;
    }

    internal override GeometryBase DuplicateShallowHelper()
    {
      return new Mesh(IntPtr.Zero, null, null);
    }
    #endregion

    #region constants
    internal const int idxUnitizeVertexNormals = 0;
    internal const int idxUnitizeFaceNormals = 1;
    internal const int idxConvertQuadsToTriangles = 2;
    internal const int idxComputeFaceNormals = 3;
    internal const int idxCompact = 4;
    internal const int idxComputeVertexNormals = 5;
    internal const int idxNormalizeTextureCoordinates = 6;
    internal const int idxTransposeTextureCoordinates = 7;
    internal const int idxTransposeSurfaceParameters = 8;

    internal const int idxVertexCount = 0;
    internal const int idxFaceCount = 1;
    internal const int idxQuadCount = 2;
    internal const int idxTriangleCount = 3;
    internal const int idxHiddenVertexCount = 4;
    const int idxDisjointMeshCount = 5;
    internal const int idxFaceNormalCount = 6;
    internal const int idxNormalCount = 7;
    internal const int idxColorCount = 8;
    internal const int idxTextureCoordinateCount = 9;
    internal const int idxMeshTopologyVertexCount = 10;

    internal const int idxHasVertexNormals = 0;
    internal const int idxHasFaceNormals = 1;
    internal const int idxHasTextureCoordinates = 2;
    internal const int idxHasSurfaceParameters = 3;
    internal const int idxHasPrincipalCurvatures = 4;
    internal const int idxHasVertexColors = 5;
    const int idxIsClosed = 6;

    internal const int idxClearVertices = 0;
    internal const int idxClearFaces = 1;
    internal const int idxClearNormals = 2;
    internal const int idxClearFaceNormals = 3;
    internal const int idxClearColors = 4;
    internal const int idxClearTextureCoordinates = 5;
    internal const int idxClearHiddenVertices = 6;

    internal const int idxHideVertex = 0;
    internal const int idxShowVertex = 1;
    internal const int idxHideAll = 2;
    internal const int idxShowAll = 3;
    internal const int idxEnsureHiddenList = 4;
    internal const int idxCleanHiddenList = 5;
    #endregion

    #region properties
    /// <summary>
    /// Gets the number of disjoint (topologically unconnected) pieces in this mesh.
    /// </summary>
    public int DisjointMeshCount
    {
      get
      {
        IntPtr ptr = ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, idxDisjointMeshCount);
      }
    }

    /// <summary>
    /// A mesh is considered to be closed when every mesh "edge" has 
    /// two or more faces.
    /// </summary>
    /// <returns>True if the mesh is closed, false if it is not.</returns>
    public bool IsClosed
    {
      get
      {
        IntPtr ptr = ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetBool(ptr, idxIsClosed);
      }
    }

    /// <summary>
    /// Gets a value indicating whether or not the mesh is manifold. 
    /// A manifold mesh does not have any "edges" that are part of three or more faces.
    /// </summary>
    /// <param name="topologicalTest">
    /// If true, the query treats coincident vertices as the same.
    /// </param>
    /// <param name="isOriented">
    /// isOriented will be set to True if the mesh is a manifold 
    /// and adjacent faces have compatible face normals.
    /// </param>
    /// <param name="hasBoundary">
    /// hasBoundary will be set to True if the mesh is a manifold 
    /// and there is at least one "edge" with no more than one adjacent face.
    /// </param>
    /// <returns>True if every mesh "edge" has at most two adjacent faces.</returns>
    public bool IsManifold(bool topologicalTest, out bool isOriented, out bool hasBoundary)
    {
      isOriented = false;
      hasBoundary = false;
      IntPtr ptr = ConstPointer();
      return UnsafeNativeMethods.ON_Mesh_IsManifold(ptr, topologicalTest, ref isOriented, ref hasBoundary);
    }

    #region fake list access
    private Rhino.Geometry.Collections.MeshVertexList m_vertices;
    /// <summary>
    /// Gets access to the Vertices of this mesh.
    /// </summary>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public Rhino.Geometry.Collections.MeshVertexList Vertices
    {
      get
      {
        if (null == m_vertices)
          m_vertices = new Rhino.Geometry.Collections.MeshVertexList(this);

        return m_vertices;
      }
    }

    private Rhino.Geometry.Collections.MeshTopologyVertexList m_topology_vertices;
    public Rhino.Geometry.Collections.MeshTopologyVertexList TopologyVertices
    {
      get
      {
        if (null == m_topology_vertices)
          m_topology_vertices = new Rhino.Geometry.Collections.MeshTopologyVertexList(this);
        return m_topology_vertices;
      }
    }

    private Rhino.Geometry.Collections.MeshVertexNormalList m_normals;
    /// <summary>
    /// Gets access to the vertex normal vectors of this mesh.
    /// </summary>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public Rhino.Geometry.Collections.MeshVertexNormalList Normals
    {
      get
      {
        if (null == m_normals)
          m_normals = new Rhino.Geometry.Collections.MeshVertexNormalList(this);

        return m_normals;
      }
    }

    private Rhino.Geometry.Collections.MeshFaceList m_faces;
    /// <summary>
    /// Gets access to the Faces of this mesh.
    /// </summary>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public Rhino.Geometry.Collections.MeshFaceList Faces
    {
      get
      {
        if (null == m_faces)
          m_faces = new Rhino.Geometry.Collections.MeshFaceList(this);

        return m_faces;
      }
    }

    private Rhino.Geometry.Collections.MeshFaceNormalList m_facenormals;
    /// <summary>
    /// Gets access to the Face normals of this mesh.
    /// </summary>
    public Rhino.Geometry.Collections.MeshFaceNormalList FaceNormals
    {
      get
      {
        if (null == m_facenormals)
          m_facenormals = new Rhino.Geometry.Collections.MeshFaceNormalList(this);

        return m_facenormals;
      }
    }

    private Rhino.Geometry.Collections.MeshVertexColorList m_vertexcolors;
    /// <summary>
    /// Gets access to the (optional) vertex colors of this mesh
    /// </summary>
    public Rhino.Geometry.Collections.MeshVertexColorList VertexColors
    {
      get
      {
        if (null == m_vertexcolors)
          m_vertexcolors = new Rhino.Geometry.Collections.MeshVertexColorList(this);
        return m_vertexcolors;
      }
    }

    private Rhino.Geometry.Collections.MeshTextureCoordinateList m_texcoords;
    /// <summary>
    /// Gets access to the Vertex texture coordinates of this mesh.
    /// </summary>
    public Rhino.Geometry.Collections.MeshTextureCoordinateList TextureCoordinates
    {
      get
      {
        if (null == m_texcoords)
          m_texcoords = new Rhino.Geometry.Collections.MeshTextureCoordinateList(this);

        return m_texcoords;
      }
    }

    #endregion
    #endregion properties

    #region methods
    /// <summary>
    /// Removes any unreferenced objects from arrays, reindexes as needed 
    /// and shrinks arrays to minimum required size.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public bool Compact()
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, idxCompact);
    }

    /// <summary>Reverses the direction of the mesh.</summary>
    /// <param name="vertexNormals">If True, vertex normals will be reversed.</param>
    /// <param name="faceNormals">If True, face normals will be reversed.</param>
    /// <param name="faceOrientation">If True, face orientations will be reversed.</param>
    public void Flip(bool vertexNormals, bool faceNormals, bool faceOrientation)
    {
      IntPtr ptr = NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_Flip(ptr, vertexNormals, faceNormals, faceOrientation);
    }

    /// <summary>
    /// Attempts to fix inconsistencies in the directions of meshfaces for a mesh
    /// </summary>
    /// <returns>number of faces that were modified</returns>
    public int UnifyNormals()
    {
      int rc = 0;
      if (IsDocumentControlled)
      {
        IntPtr pConstThis = ConstPointer();
        rc = UnsafeNativeMethods.RHC_RhinoUnifyMeshNormals(pConstThis, true);
        if (rc < 1)
          return 0;
      }

      IntPtr pThis = NonConstPointer();
      rc = UnsafeNativeMethods.RHC_RhinoUnifyMeshNormals(pThis, false);
      return rc;
    }

    /// <summary>
    /// Splits up the mesh into its unconnected pieces
    /// </summary>
    /// <returns>An array containing all the disjoint pieces that make up this Mesh.</returns>
    public Mesh[] SplitDisjointPieces()
    {
      IntPtr pConstThis = ConstPointer();
      IntPtr pMeshArray = UnsafeNativeMethods.ON_MeshArray_New();
      int count = UnsafeNativeMethods.RHC_RhinoSplitDisjointMesh(pConstThis, pMeshArray);
      Mesh[] rc = null;
      if (count > 0)
        rc = new Mesh[count];
      for (int i = 0; i < count; i++)
      {
        IntPtr pMesh = UnsafeNativeMethods.ON_MeshArray_Get(pMeshArray, i);
        rc[i] = new Mesh(pMesh, null, null);
      }
      UnsafeNativeMethods.ON_MeshArray_Delete(pMeshArray);
      return rc;
    }

    /// <summary>
    /// Split a mesh by an infinite plane
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    public Mesh[] Split(Plane plane)
    {
      IntPtr pConstThis = ConstPointer();
      IntPtr pMeshArray = UnsafeNativeMethods.ON_MeshArray_New();
      int count = UnsafeNativeMethods.RHC_RhinoMeshBooleanSplit(pConstThis, pMeshArray, ref plane);
      Mesh[] rc = null;
      if (count > 0)
        rc = new Mesh[count];
      for (int i = 0; i < count; i++)
      {
        IntPtr pMesh = UnsafeNativeMethods.ON_MeshArray_Get(pMeshArray, i);
        rc[i] = new Mesh(pMesh, null, null);
      }
      UnsafeNativeMethods.ON_MeshArray_Delete(pMeshArray);
      return rc;
    }

    /// <summary>
    /// Create outlines of a mesh projected against a plane
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    public Polyline[] GetOutlines(Plane plane)
    {
      IntPtr pConstMesh = ConstPointer();
      int polylines_created = 0;
      IntPtr pPolys = UnsafeNativeMethods.TL_GetMeshOutline(pConstMesh, ref plane, ref polylines_created);
      if (polylines_created < 1 || IntPtr.Zero == pPolys)
        return null;

      // convert the C++ polylines created into .NET polylines
      Polyline[] rc = new Polyline[polylines_created];
      for (int i = 0; i < polylines_created; i++)
      {
        int point_count = UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetCount(pPolys, i);
        Polyline pl = new Polyline(point_count);
        if (point_count > 0)
        {
          pl.m_size = point_count;
          UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetPoints(pPolys, i, point_count, pl.m_items);
        }
        rc[i] = pl;
      }
      UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_Delete(pPolys, true);

      return rc;
    }

    /// <summary>
    /// Create outlines of a mesh. The projection information in the
    /// viewport is used to determine how the outlines are projected
    /// </summary>
    /// <param name="viewport"></param>
    /// <returns></returns>
    public Polyline[] GetOutlines(Rhino.Display.RhinoViewport viewport)
    {
      IntPtr pConstMesh = ConstPointer();
      int polylines_created = 0;
      IntPtr pConstRhinoViewport = viewport.ConstPointer();
      IntPtr pPolys = UnsafeNativeMethods.TL_GetMeshOutline2(pConstMesh, pConstRhinoViewport, ref polylines_created);
      if (polylines_created < 1 || IntPtr.Zero == pPolys)
        return null;

      // convert the C++ polylines created into .NET polylines
      Polyline[] rc = new Polyline[polylines_created];
      for (int i = 0; i < polylines_created; i++)
      {
        int point_count = UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetCount(pPolys, i);
        Polyline pl = new Polyline(point_count);
        if (point_count > 0)
        {
          pl.m_size = point_count;
          UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetPoints(pPolys, i, point_count, pl.m_items);
        }
        rc[i] = pl;
      }
      UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_Delete(pPolys, true);

      return rc;
    }

    /// <summary>
    /// Returns all edges of a mesh that are considered "naked" in the
    /// sense that the edge only has one face
    /// </summary>
    /// <returns></returns>
    public Polyline[] GetNakedEdges()
    {
      IntPtr pConstThis = ConstPointer();
      int polylines_created = 0;
      IntPtr pPolys = UnsafeNativeMethods.ON_Mesh_GetNakedEdges(pConstThis, ref polylines_created);
      if (polylines_created < 1 || IntPtr.Zero == pPolys)
        return null;

      // convert the C++ polylines created into .NET polylines
      Polyline[] rc = new Polyline[polylines_created];
      for (int i = 0; i < polylines_created; i++)
      {
        int point_count = UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetCount(pPolys, i);
        Polyline pl = new Polyline(point_count);
        if (point_count > 0)
        {
          pl.m_size = point_count;
          UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_GetPoints(pPolys, i, point_count, pl.m_items);
        }
        rc[i] = pl;
      }
      UnsafeNativeMethods.ON_SimpleArray_PolylineCurve_Delete(pPolys, true);
      return rc;
    }

    /// <summary>
    /// Explode the mesh into submeshes where a submesh is a collection of faces that are contained
    /// within a closed loop of "unwelded" edges. Unwelded edges are edges where the faces that share
    /// the edge have unique mesh vertexes (not mesh topology vertexes) at both ends of the edge.
    /// </summary>
    /// <returns>
    /// Array of submeshes on success; null on error. If the count in the returned array is 1, then
    /// nothing happened and the ouput is essentially a copy of the input.
    /// </returns>
    public Mesh[] ExplodeAtUnweldedEdges()
    {     
      IntPtr pConstThis = ConstPointer();
      IntPtr pMeshArray = UnsafeNativeMethods.ON_MeshArray_New();
      int count = UnsafeNativeMethods.RHC_RhinoExplodeMesh(pConstThis, pMeshArray);
      Mesh[] rc = null;
      if (count > 0)
        rc = new Mesh[count];
      for (int i = 0; i < count; i++)
      {
        IntPtr pMesh = UnsafeNativeMethods.ON_MeshArray_Get(pMeshArray, i);
        rc[i] = new Mesh(pMesh, null, null);
      }
      UnsafeNativeMethods.ON_MeshArray_Delete(pMeshArray);
      return rc;
    }

    /// <summary>
    /// Appends a copy of another mesh to this one and updates indices of appended mesh parts.
    /// </summary>
    /// <param name="other">Mesh to append to this one.</param>
    public void Append(Mesh other)
    {
      if (null == other || other.ConstPointer() == this.ConstPointer())
        return;
      IntPtr ptr = NonConstPointer();
      IntPtr otherPtr = other.ConstPointer();
      UnsafeNativeMethods.ON_Mesh_Append(ptr, otherPtr);
    }

    /// <summary>
    /// This method is Obsolete, use ClosestPoint() instead.
    /// </summary>
    [Obsolete("This method is Obsolete, use ClosestPoint() instead.")]
    public Point3d GetClosestPoint(Point3d testPoint)
    {
      return ClosestPoint(testPoint);
    }
    /// <summary>
    /// Get the point on the mesh that is closest to a given test point.
    /// </summary>
    /// <param name="testPoint">Point to seach for.</param>
    /// <returns>The point on the mesh closest to testPoint, or Point3d.Unset on failure.</returns>
    public Point3d ClosestPoint(Point3d testPoint)
    {
      Point3d pointOnMesh;
      if (ClosestPoint(testPoint, out pointOnMesh, 0.0) < 0)
      {
        return Point3d.Unset;
      }
      else
      {
        return pointOnMesh;
      }
    }

    /// <summary>
    /// This method is Obsolete, use ClosestPoint() instead.
    /// </summary>
    [Obsolete("This method is Obsolete, use ClosestPoint() instead.")]
    public int GetClosestPoint(Point3d testPoint, out Point3d pointOnMesh, double maximumDistance)
    {
      return ClosestPoint(testPoint, out pointOnMesh, maximumDistance);
    }
    /// <summary>
    /// Get the point on the mesh that is closest to a given test point.
    /// </summary>
    /// <param name="testPoint">Point to seach for.</param>
    /// <param name="pointOnMesh">Point on the mesh closest to testPoint.</param>
    /// <param name="maximumDistance">
    /// Optional upper bound on the distance from test point to the mesh. 
    /// If you are only interested in finding a point Q on the mesh when 
    /// testPoint.DistanceTo(Q) &lt; maximumDistance, 
    /// then set maximumDistance to that value. 
    /// This parameter is ignored if you pass 0.0 for a maximumDistance.
    /// </param>
    /// <returns>
    /// Index of face that the closest point lies on if successful. 
    /// -1 if not successful; the value of pointOnMesh is undefined.
    /// </returns>
    public int ClosestPoint(Point3d testPoint, out Point3d pointOnMesh, double maximumDistance)
    {
      pointOnMesh = Point3d.Unset;
      IntPtr ptr = ConstPointer();
      return UnsafeNativeMethods.ON_Mesh_GetClosestPoint(ptr, testPoint, ref pointOnMesh, maximumDistance);
    }

    /// <summary>
    /// This method is Obsolete, use ClosestPoint() instead.
    /// </summary>
    [Obsolete("This method is Obsolete, use ClosestPoint() instead.")]
    public int GetClosestPoint(Point3d testPoint, out Point3d pointOnMesh, out Vector3d normalAtPoint, double maximumDistance)
    {
      return ClosestPoint(testPoint, out pointOnMesh, out normalAtPoint, maximumDistance);
    }
    /// <summary>
    /// Get the point on the mesh that is closest to a given test point.
    /// </summary>
    /// <param name="testPoint">Point to seach for.</param>
    /// <param name="pointOnMesh">Point on the mesh closest to testPoint.</param>
    /// <param name="normalAtPoint">The normal vector of the mesh at the closest point.</param>
    /// <param name="maximumDistance">
    /// Optional upper bound on the distance from test point to the mesh. 
    /// If you are only interested in finding a point Q on the mesh when 
    /// testPoint.DistanceTo(Q) &lt; maximumDistance, 
    /// then set maximumDistance to that value. 
    /// This parameter is ignored if you pass 0.0 for a maximumDistance.
    /// </param>
    /// <returns>
    /// Index of face that the closest point lies on if successful. 
    /// -1 if not successful; the value of pointOnMesh is undefined.
    /// </returns>
    public int ClosestPoint(Point3d testPoint, out Point3d pointOnMesh, out Vector3d normalAtPoint, double maximumDistance)
    {
      pointOnMesh = Point3d.Unset;
      normalAtPoint = Vector3d.Unset;
      IntPtr ptr = ConstPointer();
      return UnsafeNativeMethods.ON_Mesh_GetClosestPoint2(ptr, testPoint, ref pointOnMesh, ref normalAtPoint, maximumDistance);
    }

    /// <summary>
    /// Pull a collection of points to a mesh.
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public Point3d[] PullPointsToMesh(IEnumerable<Point3d> points)
    {
      List<Point3d> rc = new List<Point3d>();
      foreach (Point3d point in points)
      {
        Point3d closest = ClosestPoint(point);
        if (closest.IsValid)
          rc.Add(closest);
      }
      return rc.ToArray();
    }

    /// <summary>
    /// Make a new mesh with vertices offset a distance in the opposite direction of the existing vertex normals.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns>new mesh on success, null on failure</returns>
    public Mesh Offset(double distance)
    {
      IntPtr pConstMesh = ConstPointer();
      IntPtr pNewMesh = UnsafeNativeMethods.RHC_RhinoOffsetMesh(pConstMesh, distance);
      if (IntPtr.Zero == pNewMesh)
        return null;
      return new Mesh(pNewMesh, null, null);
    }
    #endregion


    ///// <summary>
    ///// Gets a value indicating whether or not the mesh has nurbs surface parameters.
    ///// </summary>
    //public bool HasSurfaceParameters
    //{
    //  get
    //  {
    //    IntPtr ptr = ConstPointer();
    //    return UnsafeNativeMethods.ON_Mesh_GetBool(ptr, idxHasSurfaceParameters);
    //  }
    //}

    ///// <summary>
    ///// Gets a value indicating whether or not the mesh has nurbs surface curvature data.
    ///// </summary>
    //public bool HasPrincipalCurvatures
    //{
    //  get
    //  {
    //    IntPtr ptr = ConstPointer();
    //    return UnsafeNativeMethods.ON_Mesh_GetBool(ptr, idxHasPrincipalCurvatures);
    //  }
    //}

    // [skipping]
    //  ON_MeshVertexRef VertexRef(ON_ComponentIndex ci) const;
    //  ON_MeshVertexRef VertexRef(int mesh_V_index) const;
    //  ON_MeshEdgeRef EdgeRef(ON_ComponentIndex ci) const;
    //  ON_MeshEdgeRef EdgeRef(int tope_index) const;
    //  ON_MeshFaceRef FaceRef(ON_ComponentIndex ci) const;
    //  ON_MeshFaceRef FaceRef(int mesh_F_index) const;
    //  ON_Geometry* MeshComponent( 
    //  bool GetCurvatureStats( 
    //  void InvalidateVertexBoundingBox(); // Call if defining geometry is changed by 
    //  void InvalidateVertexNormalBoundingBox(); // Call if defining geometry is changed by 
    //  void InvalidateTextureCoordinateBoundingBox(); // Call if defining geometry is changed by 
    //  void InvalidateCurvatureStats(); // Call if defining geometry is changed by 
    //  void InvalidateBoundingBoxes(); // Invalidates all cached bounding box information.
    //  void SetMeshParameters( const ON_MeshParameters& );
    //  const ON_MeshParameters* MeshParameters() const;
    //  void DeleteMeshParameters();

    #region topological methods
    /// <summary>
    /// Returns an array of bool values equal in length to the number of vertices in this
    /// mesh. Each value corresponds to a mesh vertex and is set to true if the vertex is
    /// not completely surrounded by faces.
    /// </summary>
    /// <returns></returns>
    public bool[] GetNakedEdgePointStatus()
    {
      int count = this.Vertices.Count;
      if (count < 1)
        return null;

      // IMPORTANT!!! - DO NOT marshal arrays of bools. This can cause problems with
      // the marshaler because it will attempt to convert the items into U1 size
      int[] status = new int[count];
      IntPtr pThis = ConstPointer();
      if (UnsafeNativeMethods.ON_Mesh_NakedEdgePoints(pThis, status, count))
      {
        bool[] rc = new bool[count];
        for (int i = 0; i < count; i++)
        {
          if (status[i] != 0)
            rc[i] = true;
        }
        return rc;
      }
      return null;
    }

    //David: I have disabled these. It seems very, very geeky.
    //public bool TransposeSurfaceParameters()
    //{
    //  IntPtr ptr = NonConstPointer();
    //  return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, idxTransposeSurfaceParameters);
    //}

    //public bool ReverseSurfaceParameters(int direction)
    //{
    //  IntPtr ptr = NonConstPointer();
    //  return UnsafeNativeMethods.ON_Mesh_Reverse(ptr, false, direction);
    //}

    ///// <summary>
    ///// finds all coincident vertices and merges them if break angle is small enough
    ///// </summary>
    ///// <param name="tolerance">coordinate tols for considering vertices to be coincident</param>
    ///// <param name="cosineNormalAngle">
    ///// cosine normal angle tolerance in radians
    ///// if vertices are coincident, then they are combined
    ///// if NormalA o NormalB >= this value
    ///// </param>
    ///// <returns></returns>
    //public bool CombineCoincidentVertices(Vector3f tolerance, double cosineNormalAngle)
    //{
    //  IntPtr ptr = NonConstPointer();
    //  return UnsafeNativeMethods.ON_Mesh_CombineCoincidentVertices(ptr, tolerance, cosineNormalAngle);
    //}

    #endregion



    //[skipping]
    //  bool SetTextureCoordinates( 
    //  bool HasCachedTextureCoordinates() const;
    //  const ON_TextureCoordinates* CachedTextureCoordinates( 
    //  const ON_TextureCoordinates* SetCachedTextureCoordinates( 
    //  bool EvaluateMeshGeometry( const ON_Surface& ); // evaluate surface at tcoords
    //  int GetVertexEdges( 
    //  int GetMeshEdges( 

    //[skipping]
    // other versions of get closest point that returns other parts of ON_MESH_POINT
    //  bool GetClosestPoint( const ON_3dPoint& P, ON_MESH_POINT* Q, double maximumDistance = 0.0 ) const;

    // [skipping]
    //  int IntersectMesh( const ON_Mesh& meshB, ON_ClassArray< ON_SimpleArray< ON_MMX_POINT > >& x, double intersection_tolerance = 0.0, double overlap_tolerance = 0.0 ) const;

    // [skipping]
    //  const class ON_MeshTree* MeshTree() const;
    //  void DestroyTree( bool bDeleteTree = true );
    //[skipping]
    // bool VolumeMassProperties(
    // bool CollapseEdge( int topei );
    // bool IsSwappableEdge( int topei );
    // bool SwapEdge( int topei );
    //  void DestroyHiddenVertexArray();
    //  const bool* HiddenVertexArray() const;
    //  void SetVertexHiddenFlag( int meshvi, bool bHidden );
    //  bool VertexIsHidden( int meshvi ) const;
    //  bool FaceIsHidden( int meshvi ) const;
    //  const ON_MeshTopology& Topology() const;
    //  void DestroyTopology();
    //  const ON_MeshPartition* CreatePartition( 
    //  const ON_MeshPartition* Partition() const;
    //  void DestroyPartition();
    //  const class ON_MeshNgonList* NgonList() const;
    //  class ON_MeshNgonList* ModifyNgonList();
    //  void DestroyNgonList();


    // [skipping]
    //  ON_3fVectorArray m_N;
    //  ON_3fVectorArray m_FN;
    //  ON_MappingTag m_Ttag; // OPTIONAL tag for values in m_T[]
    //  ON_2fPointArray m_T;  // OPTIONAL texture coordinates for each vertex
    //  ON_2dPointArray m_S;
    //  ON_Interval m_srf_domain[2]; // surface evaluation domain.
    //  double m_srf_scale[2];
    //  ON_Interval m_packed_tex_domain[2];
    //  bool m_packed_tex_rotate;
    //  bool HasPackedTextureRegion() const;
    //  ON_SimpleArray<ON_SurfaceCurvature> m_K;  // OPTIONAL surface curvatures
    //  ON_MappingTag m_Ctag; // OPTIONAL tag for values in m_C[]
    //  ON_SimpleArray<ON_Color> m_C
    //  ON_SimpleArray<bool> m_H; // OPTIONAL vertex visibility.
    //  int m_hidden_count;       // number of vertices that are hidden
    //  const ON_Object* m_parent; // runtime parent geometry (use ...::Cast() to get it)
  }
}

namespace Rhino.Geometry.Collections
{
  /// <summary>
  /// Provides access to the Vertices and Vertex related functionality of a Mesh.
  /// </summary>
  public class MeshVertexList : IEnumerable<Point3f>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshVertexList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh vertices.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxVertexCount);
      }
      set
      {
        if (value >= 0 && value != Count)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxVertexCount, value);
          UnsafeNativeMethods.ON_Mesh_RepairHiddenArray(pMesh);
        }
      }
    }

    /// <summary>
    /// Gets or sets the vertex at the given index. 
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of control vertex to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The control vertex at [index].</returns>
    public Point3f this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        Point3f rc = new Point3f();
        IntPtr ptr = m_mesh.ConstPointer();
        UnsafeNativeMethods.ON_Mesh_Vertex(ptr, index, ref rc);
        return rc;
      }
      set
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetVertex(ptr, index, value.m_x, value.m_y, value.m_z);
      }
    }
    #endregion

    #region access
    /// <summary>
    /// Clear the Vertex list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearVertices);
      UnsafeNativeMethods.ON_Mesh_RepairHiddenArray(ptr);
    }

    /// <summary>
    /// Adds a new vertex to the end of the Vertex list.
    /// </summary>
    /// <param name="x">X component of new vertex coordinate.</param>
    /// <param name="y">Y component of new vertex coordinate.</param>
    /// <param name="z">Z component of new vertex coordinate.</param>
    /// <returns>The index of the newly added vertex.</returns>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public int Add(float x, float y, float z)
    {
      int count = Count;
      SetVertex(count, x, y, z);

      UnsafeNativeMethods.ON_Mesh_RepairHiddenArray(m_mesh.NonConstPointer());

      return count;
    }
    /// <summary>
    /// Adds a new vertex to the end of the Vertex list.
    /// </summary>
    /// <param name="x">X component of new vertex coordinate.</param>
    /// <param name="y">Y component of new vertex coordinate.</param>
    /// <param name="z">Z component of new vertex coordinate.</param>
    /// <returns>The index of the newly added vertex.</returns>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public int Add(double x, double y, double z)
    {
      return Add((float)x, (float)y, (float)z);
    }
    /// <summary>
    /// Adds a new vertex to the end of the Vertex list.
    /// </summary>
    /// <param name="vertex">Location of new vertex.</param>
    /// <returns>The index of the newly added vertex.</returns>
    public int Add(Point3f vertex)
    {
      return Add(vertex.m_x, vertex.m_y, vertex.m_z);
    }
    /// <summary>
    /// Adds a new vertex to the end of the Vertex list.
    /// </summary>
    /// <param name="vertex">Location of new vertex.</param>
    /// <returns>The index of the newly added vertex.</returns>
    public int Add(Point3d vertex)
    {
      return Add((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
    }

    public void AddVertices(IEnumerable<Point3d> vertices)
    {
      IntPtr pMesh = m_mesh.NonConstPointer();
      int count = Count;
      int index = count;
      foreach (Point3d vertex in vertices)
      {
        float x = (float)vertex.X;
        float y = (float)vertex.Y;
        float z = (float)vertex.Z;
        UnsafeNativeMethods.ON_Mesh_SetVertex(pMesh, index, x, y, z);
        index++;
      }
      UnsafeNativeMethods.ON_Mesh_RepairHiddenArray(pMesh);
    }

    public void AddVertices(IEnumerable<Point3f> vertices)
    {
      IntPtr pMesh = m_mesh.NonConstPointer();
      int count = Count;
      int index = count;
      foreach (Point3f vertex in vertices)
      {
        float x = vertex.X;
        float y = vertex.Y;
        float z = vertex.Z;
        UnsafeNativeMethods.ON_Mesh_SetVertex(pMesh, index, x, y, z);
        index++;
      }
      UnsafeNativeMethods.ON_Mesh_RepairHiddenArray(pMesh);
    }

    /// <summary>
    /// Sets or adds a vertex to the Vertex List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex to set.</param>
    /// <param name="x">X component of vertex location.</param>
    /// <param name="y">Y component of vertex location.</param>
    /// <param name="z">Z component of vertex location.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetVertex(int index, float x, float y, float z)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      bool rc = UnsafeNativeMethods.ON_Mesh_SetVertex(ptr, index, x, y, z);
      return rc;
    }
    /// <summary>
    /// Sets or adds a vertex to the Vertex List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex to set.</param>
    /// <param name="x">X component of vertex location.</param>
    /// <param name="y">Y component of vertex location.</param>
    /// <param name="z">Z component of vertex location.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetVertex(int index, double x, double y, double z)
    {
      return SetVertex(index, (float)x, (float)y, (float)z);
    }
    /// <summary>
    /// Sets or adds a vertex to the Vertex List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex to set.</param>
    /// <param name="vertex">Vertex location.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetVertex(int index, Point3f vertex)
    {
      return SetVertex(index, vertex.X, vertex.Y, vertex.Z);
    }
    /// <summary>
    /// Sets or adds a vertex to the Vertex List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex to set.</param>
    /// <param name="vertex">Vertex location.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetVertex(int index, Point3d vertex)
    {
      return SetVertex(index, (float)vertex.X, (float)vertex.Y, (float)vertex.Z);
    }

    /// <summary>
    /// Gets a value indicating whether or not a vertex is hidden.
    /// </summary>
    /// <param name="vertexIndex">Index of vertex to query.</param>
    /// <returns>True if the vertex is hidden, false if it is not.</returns>
    public bool IsHidden(int vertexIndex)
    {
      return UnsafeNativeMethods.ON_Mesh_GetHiddenValue(m_mesh.ConstPointer(), vertexIndex);
    }

    /// <summary>
    /// Hide the vertex at the given index.
    /// </summary>
    /// <param name="vertexIndex">Index of vertex to hide.</param>
    public void Hide(int vertexIndex)
    {
      // If vertex is already hidden, DO NOT copy the mesh but return right away.
      if (UnsafeNativeMethods.ON_Mesh_GetHiddenValue(m_mesh.ConstPointer(), vertexIndex))
      { return; }

      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, 0, Mesh.idxEnsureHiddenList);
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, vertexIndex, Mesh.idxHideVertex);
    }
    /// <summary>
    /// Show the vertex at the given index.
    /// </summary>
    /// <param name="vertexIndex">Index of vertex to show.</param>
    public void Show(int vertexIndex)
    {
      // If vertex is already visible, DO NOT copy the mesh but return right away.
      if (!UnsafeNativeMethods.ON_Mesh_GetHiddenValue(m_mesh.ConstPointer(), vertexIndex))
      { return; }

      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, 0, Mesh.idxEnsureHiddenList);
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, vertexIndex, Mesh.idxShowVertex);
    }
    /// <summary>
    /// Hide all vertices in the mesh.
    /// </summary>
    public void HideAll()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, 0, Mesh.idxEnsureHiddenList);
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, 0, Mesh.idxHideAll);
    }
    /// <summary>
    /// Show all vertices in the mesh.
    /// </summary>
    public void ShowAll()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_HiddenVertexOp(ptr, 0, Mesh.idxShowAll);
    }
    #endregion

    #region methods
    /// <summary>
    /// Cull (remove) all vertices that are currently not used by the Face list.
    /// </summary>
    /// <returns>The number of unused vertices that were removed.</returns>
    public int CullUnused()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_CullOp(ptr, false);
    }

    /// <summary>
    /// Merge identical vertices.
    /// </summary>
    /// <param name="ignoreNormals">
    /// If true, vertex normals will not be taken into consideration when comparing vertices.
    /// </param>
    /// <param name="ignoreAdditional">
    /// If true, texture coordinates, colors, and principal curvatures 
    /// will not be taken into consideration when comparing vertices.
    /// </param>
    /// <returns>
    /// True if the mesh is changed, in which case the mesh will have fewer vertices than before.
    /// </returns>
    public bool CombineIdentical(bool ignoreNormals, bool ignoreAdditional)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_CombineIdenticalVertices(ptr, ignoreNormals, ignoreAdditional);
    }

    /// <summary>
    /// Get a list of all of the faces that share a given vertex
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <returns>list of indices of faces on success, null on failure</returns>
    public int[] GetVertexFaces(int vertexIndex)
    {
      IntPtr pConstMesh = m_mesh.ConstPointer();
      Rhino.Runtime.INTERNAL_IntArray face_ids = new Rhino.Runtime.INTERNAL_IntArray();
      int count = UnsafeNativeMethods.ON_Mesh_GetVertexFaces(pConstMesh, face_ids.m_ptr, vertexIndex);
      int[] ids = null;
      if (count > 0)
        ids = face_ids.ToArray();
      face_ids.Dispose();
      return ids;
    }

    /// <summary>
    /// Get a list of other vertices which a "topologically" identical
    /// to this vertex
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <returns>
    /// Array of indices of vertices that are topoligically the same as this vertex. The
    /// array includes vertexIndex. Returns null on failure
    /// </returns>
    public int[] GetTopologicalIndenticalVertices(int vertexIndex)
    {
      IntPtr pConstMesh = m_mesh.ConstPointer();
      int[] ids = null;
      using (Rhino.Runtime.INTERNAL_IntArray vertex_ids = new Rhino.Runtime.INTERNAL_IntArray())
      {
        int count = UnsafeNativeMethods.ON_Mesh_GetTopologicalVertices(pConstMesh, vertex_ids.m_ptr, vertexIndex);
        if (count > 0)
          ids = vertex_ids.ToArray();
      }
      return ids;
    }

    /// <summary>
    /// Gets indices of all vertices that form "edges" with a given vertex index.
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <returns></returns>
    public int[] GetConnectedVertices(int vertexIndex)
    {
      IntPtr pConstMesh = m_mesh.ConstPointer();
      int[] ids = null;
      using (Rhino.Runtime.INTERNAL_IntArray vertex_ids = new Rhino.Runtime.INTERNAL_IntArray())
      {
        int count = UnsafeNativeMethods.ON_Mesh_GetConnectedVertices(pConstMesh, vertex_ids.m_ptr, vertexIndex);
        if (count > 0)
          ids = vertex_ids.ToArray();
      }
      return ids;
    }

    /// <summary>
    /// Copy all of the points in this vertex list to an array of Point3d
    /// </summary>
    /// <returns></returns>
    public Point3f[] ToPoint3fArray()
    {
      int count = Count;
      Point3f[] rc = new Point3f[count];
      IntPtr pConstMesh = m_mesh.ConstPointer();
      for (int i = 0; i < count; i++)
      {
        Point3f pt = new Point3f();
        UnsafeNativeMethods.ON_Mesh_Vertex(pConstMesh, i, ref pt);
        rc[i] = pt;
      }
      return rc;
    }

    public Point3d[] ToPoint3dArray()
    {
      int count = Count;
      Point3d[] rc = new Point3d[count];
      IntPtr pConstMesh = m_mesh.ConstPointer();
      for (int i = 0; i < count; i++)
      {
        Point3f pt = new Point3f();
        UnsafeNativeMethods.ON_Mesh_Vertex(pConstMesh, i, ref pt);
        rc[i] = new Point3d(pt);
      }
      return rc;
    }
    #endregion

    #region IEnumerable implementation
    IEnumerator<Point3f> IEnumerable<Point3f>.GetEnumerator()
    {
      return new MVEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MVEnum(this);
    }

    private class MVEnum : IEnumerator<Point3f>
    {
      #region members
      private MeshVertexList m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MVEnum(MeshVertexList mesh_vertices)
      {
        m_owner = mesh_vertices;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Point3f Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed) { return; }
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion
  }

  /// <summary>
  /// Provides access to the mesh topology vertices of a mesh. Topology vertices are
  /// sets of vertices in the MeshVertexList that can topologically be considered the
  /// same vertex
  /// </summary>
  public class MeshTopologyVertexList : IEnumerable<Point3f>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshTopologyVertexList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh topology vertices.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxMeshTopologyVertexCount);
      }
    }

    /// <summary>
    /// Gets or sets the vertex at the given index. Setting a location adjusts all vertices
    /// in the mesh's vertex list that are defined by this topological vertex
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of topology vertex to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The topological vertex at [index].</returns>
    public Point3f this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        Point3f rc = new Point3f();
        IntPtr ptr = m_mesh.ConstPointer();
        UnsafeNativeMethods.ON_Mesh_TopologyVertex(ptr, index, ref rc);
        return rc;
      }
      set
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetTopologyVertex(ptr, index, value);
      }
    }
    #endregion

    #region IEnumerable implementation
    IEnumerator<Point3f> IEnumerable<Point3f>.GetEnumerator()
    {
      return new MTVEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MTVEnum(this);
    }

    private class MTVEnum : IEnumerator<Point3f>
    {
      #region members
      private MeshTopologyVertexList m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MTVEnum(MeshTopologyVertexList mesh_vertices)
      {
        m_owner = mesh_vertices;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Point3f Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed)
          return;
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion

  }

  /// <summary>
  /// Provides access to the Vertex Normals of a Mesh.
  /// </summary>
  public class MeshVertexNormalList //: IEnumerable<Vector3f> Steve - Hold off on making this enumberable for now
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshVertexNormalList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh vertex normals.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxNormalCount);
      }
      set
      {
        if (value >= 0 && Count != value)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxNormalCount, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the vertex at the given index. 
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of control vertex to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The control vertex at [index].</returns>
    public Vector3f this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        Vector3f rc = new Vector3f();
        IntPtr ptr = m_mesh.ConstPointer();
        UnsafeNativeMethods.ON_Mesh_GetNormal(ptr, index, ref rc, false);
        return rc;
      }
      set
      {
        if (index < 0 || index >= Count)
        {
          throw new IndexOutOfRangeException();
        }

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetNormal(ptr, index, value, false);
      }
    }
    #endregion

    #region access
    /// <summary>
    /// Clear the Vertex Normal list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearNormals);
    }

    private bool SetNormalsHelper(Vector3f[] normals, bool append)
    {
      if (null == normals || normals.Length < 1)
        return false;
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetNormals(ptr, normals.Length, normals, append);
    }

    /// <summary>
    /// Adds a new vertex normal to the end of the list.
    /// </summary>
    /// <param name="x">X component of new vertex normal.</param>
    /// <param name="y">Y component of new vertex normal.</param>
    /// <param name="z">Z component of new vertex normal.</param>
    /// <returns>The index of the newly added vertex normal.</returns>
    public int Add(float x, float y, float z)
    {
      return Add(new Vector3f(x, y, z));
    }
    /// <summary>
    /// Adds a new vertex normal to the end of the list.
    /// </summary>
    /// <param name="x">X component of new vertex normal.</param>
    /// <param name="y">Y component of new vertex normal.</param>
    /// <param name="z">Z component of new vertex normal.</param>
    /// <returns>The index of the newly added vertex normal.</returns>
    public int Add(double x, double y, double z)
    {
      return Add(new Vector3f((float)x, (float)y, (float)z));
    }
    /// <summary>
    /// Adds a new vertex normal to the end of the list.
    /// </summary>
    /// <param name="normal">new vertex normal.</param>
    /// <returns>The index of the newly added vertex normal.</returns>
    public int Add(Vector3f normal)
    {
      int N = Count;
      if (!SetNormal(N, normal)) { return -1; }
      return N;
    }
    /// <summary>
    /// Adds a new vertex normal to the end of the list.
    /// </summary>
    /// <param name="normal">new vertex normal.</param>
    /// <returns>The index of the newly added vertex normal.</returns>
    public int Add(Vector3d normal)
    {
      return Add(new Vector3f((float)normal.X, (float)normal.Y, (float)normal.Z));
    }
    /// <summary>
    /// Append a collection of normal vectors.
    /// </summary>
    /// <param name="normals">Normals to append.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool AddRange(Vector3f[] normals)
    {
      return SetNormalsHelper(normals, true);
    }

    /// <summary>
    /// Sets or adds a normal to the list.
    /// <para>If [index] is less than [Count], the existing vertex normal at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex normal is appended to the end of the list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex normal to set.</param>
    /// <param name="x">X component of vertex normal.</param>
    /// <param name="y">Y component of vertex normal.</param>
    /// <param name="z">Z component of vertex normal.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetNormal(int index, float x, float y, float z)
    {
      return SetNormal(index, new Vector3f(x, y, z));
    }
    /// <summary>
    /// Sets or adds a vertex normal to the list.
    /// <para>If [index] is less than [Count], the existing vertex normal at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex normal is appended to the end of the list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex normal to set.</param>
    /// <param name="x">X component of vertex normal.</param>
    /// <param name="y">Y component of vertex normal.</param>
    /// <param name="z">Z component of vertex normal.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetNormal(int index, double x, double y, double z)
    {
      return SetNormal(index, new Vector3f((float)x, (float)y, (float)z));
    }
    /// <summary>
    /// Sets or adds a vertex normal to the list.
    /// <para>If [index] is less than [Count], the existing vertex normal at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex normal is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex normal to set.</param>
    /// <param name="normal"></param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetNormal(int index, Vector3f normal)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetNormal(ptr, index, normal, false);
    }
    /// <summary>
    /// Sets or adds a vertex normal to the list.
    /// <para>If [index] is less than [Count], the existing vertex normal at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex normal is appended to the end of the list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex normal to set.</param>
    /// <param name="normal"></param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetNormal(int index, Vector3d normal)
    {
      return SetNormal(index, new Vector3f((float)normal.m_x, (float)normal.m_y, (float)normal.m_z));
    }
    /// <summary>
    /// Set all normal vectors in one go. This method destroys the current normal array if it exists.
    /// </summary>
    /// <param name="normals">Normals for the entire mesh.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetNormals(Vector3f[] normals)
    {
      return SetNormalsHelper(normals, false);
    }
    #endregion

    #region methods
    /// <summary>
    /// Computes the vertex normals based on the physical shape of the mesh.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public bool ComputeNormals()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxComputeVertexNormals);
    }

    /// <summary>
    /// Unitize all vertex normals.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool UnitizeNormals()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxUnitizeVertexNormals);
    }
    #endregion
    /*
    #region IEnumerable implementation
    IEnumerator<Vector3f> IEnumerable<Vector3f>.GetEnumerator()
    {
      return new MNEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MNEnum(this);
    }

    private class MNEnum : IEnumerator<Vector3f>
    {
      #region members
      private MeshNormals m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MNEnum(MeshNormals mesh_vertexnormals)
      {
        m_owner = mesh_vertexnormals;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Vector3f Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed) { return; }
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion
    */
  }

  /// <summary>
  /// Provides access to the Faces and Face related functionality of a Mesh.
  /// </summary>
  public class MeshFaceList //: IEnumerable<MeshFace>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshFaceList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh faces.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxFaceCount);
      }
      set
      {
        if (value >= 0 && Count != value)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxFaceCount, value);
        }
      }
    }

    /// <summary>
    /// Gets the number of faces that are quads (4 corners).
    /// </summary>
    public int QuadCount
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxQuadCount);
      }
    }

    /// <summary>
    /// Gets the number of faces that are triangles (3 corners).
    /// </summary>
    public int TriangleCount
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxTriangleCount);
      }
    }

    // 7 Mar 2010 S. Baer - skipping indexing operator for now. I'm a little concerned that this
    // would cause code that looks like
    // int v0 = mesh.Faces[0].A;
    // int v1 = mesh.Faces[0].B;
    // int v2 = mesh.Faces[0].C;
    // int v3 = mesh.Faces[0].D;
    // The above code would always be 4 times as slow as a single call to get all 4 indices at once
    //public MeshFace this[int index]
    #endregion

    #region methods
    #region face access
    /// <summary>
    /// Clear the Face list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearFaces);
    }

    /// <summary>
    /// Append a new mesh face to the end of the mesh face list.
    /// </summary>
    /// <param name="face">Face to add.</param>
    /// <returns>The index of the newly added face.</returns>
    public int AddFace(MeshFace face)
    {
      return AddFace(face.m_a, face.m_b, face.m_c, face.m_d);
    }
    ///// <summary>
    ///// Append a new face to the end of the mesh face list.
    ///// </summary>
    ///// <param name="face">Face to append.</param>
    ///// <returns>The index of the newly added face.</returns>
    //public int AddFace(MeshFace face)
    //{
    //  return AddFace(face.m_a, face.m_b, face.m_c, face.m_d);
    //}
    /// <summary>
    /// Append a new triangular face to the end of the mesh face list.
    /// </summary>
    /// <param name="vertex1">Index of first face corner.</param>
    /// <param name="vertex2">Index of second face corner.</param>
    /// <param name="vertex3">Index of third face corner.</param>
    /// <returns>The index of the newly added triangle.</returns>
    public int AddFace(int vertex1, int vertex2, int vertex3)
    {
      return AddFace(vertex1, vertex2, vertex3, vertex3);
    }
    /// <summary>
    /// Append a new quadragular face to the end of the mesh face list.
    /// </summary>
    /// <param name="vertex1">Index of first face corner.</param>
    /// <param name="vertex2">Index of second face corner.</param>
    /// <param name="vertex3">Index of third face corner.</param>
    /// <param name="vertex4">Index of fourth face corner.</param>
    /// <returns>The index of the newly added quad.</returns>
    /// <example>
    /// <code source='examples\vbnet\ex_addmesh.vb' lang='vbnet'/>
    /// <code source='examples\cs\ex_addmesh.cs' lang='cs'/>
    /// <code source='examples\py\ex_addmesh.py' lang='py'/>
    /// </example>
    public int AddFace(int vertex1, int vertex2, int vertex3, int vertex4)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_AddFace(ptr, vertex1, vertex2, vertex3, vertex4);
    }

    public bool SetFace(int index, MeshFace face)
    {
      return SetFace(index, face.m_a, face.m_b, face.m_c, face.m_d);
    }
    public bool SetFace(int index, int vertex1, int vertex2, int vertex3)
    {
      return SetFace(index, vertex1, vertex2, vertex3, vertex3);
    }
    public bool SetFace(int index, int vertex1, int vertex2, int vertex3, int vertex4)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetFace(ptr, index, vertex1, vertex2, vertex3, vertex4);
    }

    /// <summary>
    /// Returns the mesh face at the given index. 
    /// </summary>
    /// <param name="index">Index of face to get. Must be larger than or equal to zero and 
    /// smaller than the Face Count of the mesh.</param>
    /// <returns>The mesh face at the given index on success or MeshFace.Unset if the index is out of range.</returns>
    public MeshFace GetFace(int index)
    {
      MeshFace rc = new MeshFace();
      IntPtr pMesh = m_mesh.ConstPointer();
      if (UnsafeNativeMethods.ON_Mesh_GetFace(pMesh, index, ref rc))
        return rc;

      return MeshFace.Unset;
    }

    public bool GetFaceVertices(int faceIndex, out Point3f a, out Point3f b, out Point3f c, out Point3f d)
    {
      IntPtr pMesh = m_mesh.ConstPointer();
      a = new Point3f();
      b = new Point3f();
      c = new Point3f();
      d = new Point3f();
      bool rc = UnsafeNativeMethods.ON_Mesh_GetFaceVertices(pMesh, faceIndex, ref a, ref b, ref c, ref d);
      return rc;
    }

    public Point3d GetFaceCenter(int faceIndex)
    {
      IntPtr pConstThis = m_mesh.ConstPointer();
      Point3d rc = new Point3d();
      if (!UnsafeNativeMethods.ON_Mesh_GetFaceCenter(pConstThis, faceIndex, ref rc))
        throw new IndexOutOfRangeException();
      return rc;
    }
    #endregion

    /// <summary>
    /// Removes a collection of faces from the mesh without affecting the remaining geometry.
    /// </summary>
    /// <param name="faceIndexes">An array containing all the face indices to be removed.</param>
    /// <returns>The number of faces deleted on success.</returns>
    public int DeleteFaces(IEnumerable<int> faceIndexes)
    {
      if (null == faceIndexes)
        return 0;
      RhinoList<int> _faceIndexes = new RhinoList<int>(faceIndexes);

      if (_faceIndexes.Count < 1)
        return 0;
      int[] f = _faceIndexes.m_items;
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_DeleteFace(ptr, _faceIndexes.Count, f);
    }

    /// <summary>Splits all quads along the short diagonal.</summary>
    /// <returns>True on success, false on failure.</returns>
    public bool ConvertQuadsToTriangles()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxConvertQuadsToTriangles);
    }

    /// <summary>
    /// Joins adjacent triangles into quads if the resulting quad is 'nice'.
    /// </summary>
    /// <param name="angleToleranceRadians">
    /// Used to compare adjacent triangles' face normals. For two triangles 
    /// to be considered, the angle between their face normals has to 
    /// be &lt;= angleToleranceRadians. When in doubt use RhinoMath.PI/90.0 (2 degrees).
    /// </param>
    /// <param name="minimumDiagonalLengthRatio">
    /// ( &lt;= 1.0) For two triangles to be considered the ratio of the 
    /// resulting quad's diagonals 
    /// (length of the shortest diagonal)/(length of longest diagonal). 
    /// has to be >= minimumDiagonalLengthRatio. When in doubt us .875.
    /// </param>
    /// <returns>True on success, false on failure.</returns>
    public bool ConvertTrianglesToQuads(double angleToleranceRadians, double minimumDiagonalLengthRatio)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_ConvertTrianglesToQuads(ptr, angleToleranceRadians, minimumDiagonalLengthRatio);
    }

    //David: I'm still unclear about what degenerate faces actually are, however I suspect renaming this function to 
    //CullInvalidFaces might make more sense.
    /// <summary>
    /// Cull (delete) all degenerate faces from the mesh.
    /// </summary>
    /// <returns>The number of degenerate faces that were removed.</returns>
    public int CullDegenerateFaces()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_CullOp(ptr, true);
    }
    #endregion

    /*
  #region IEnumerable implementation
  IEnumerator<MeshFace> IEnumerable<MeshFace>.GetEnumerator()
  {
    return new MFEnum(this);
  }
  IEnumerator IEnumerable.GetEnumerator()
  {
    return new MFEnum(this);
  }

  private class MFEnum : IEnumerator<MeshFace>
  {
    #region members
    private MeshFaces m_owner;
    int position = -1;
    #endregion

    #region constructor
    public MFEnum(MeshFaces mesh_faces)
    {
      m_owner = mesh_faces;
    }
    #endregion

    #region enumeration logic
    public bool MoveNext()
    {
      position++;
      return (position < m_owner.Count);
    }
    public void Reset()
    {
      position = -1;
    }

    public MeshFace Current
    {
      get
      {
        try
        {
          return m_owner[position];
        }
        catch (IndexOutOfRangeException)
        {
          throw new InvalidOperationException();
        }
      }
    }
    object IEnumerator.Current
    {
      get
      {
        try
        {
          return m_owner[position];
        }
        catch (IndexOutOfRangeException)
        {
          throw new InvalidOperationException();
        }
      }
    }
    #endregion

    #region IDisposable logic
    private bool m_disposed; // = false; <- initialized by runtime
    public void Dispose()
    {
      if (m_disposed) { return; }
      m_disposed = true;
      GC.SuppressFinalize(this);
    }
    #endregion
  }
  #endregion
  */
  }

  /// <summary>
  /// Provides access to the Face normals of a Mesh.
  /// </summary>
  public class MeshFaceNormalList //: IEnumerable<Vector3f>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshFaceNormalList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh face normals.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxFaceNormalCount);
      }
      set
      {
        if (value >= 0 && Count != value)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxFaceNormalCount, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the face normal at the given face index. 
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of face normal to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The face normal at [index].</returns>
    public Vector3f this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        Vector3f rc = new Vector3f();
        IntPtr ptr = m_mesh.ConstPointer();
        UnsafeNativeMethods.ON_Mesh_GetNormal(ptr, index, ref rc, true);
        return rc;
      }
      set
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetNormal(ptr, index, value, true);
      }
    }
    #endregion

    #region methods
    #region face access
    /// <summary>
    /// Clear the Face Normal list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearFaceNormals);
    }

    /// <summary>
    /// Append a face normal to the list of mesh face normals.
    /// </summary>
    /// <param name="x">X component of face normal.</param>
    /// <param name="y">Y component of face normal.</param>
    /// <param name="z">Z component of face normal.</param>
    /// <returns>The index of the newly added face normal.</returns>
    public int AddFaceNormal(float x, float y, float z)
    {
      return AddFaceNormal(new Vector3f(x, y, z));
    }
    /// <summary>
    /// Append a face normal to the list of mesh face normals.
    /// </summary>
    /// <param name="x">X component of face normal.</param>
    /// <param name="y">Y component of face normal.</param>
    /// <param name="z">Z component of face normal.</param>
    /// <returns>The index of the newly added face normal.</returns>
    public int AddFaceNormal(double x, double y, double z)
    {
      return AddFaceNormal(new Vector3f((float)x, (float)y, (float)z));
    }
    /// <summary>
    /// Append a face normal to the list of mesh face normals.
    /// </summary>
    /// <param name="normal">New face normal.</param>
    /// <returns>The index of the newly added face normal.</returns>
    public int AddFaceNormal(Vector3d normal)
    {
      return AddFaceNormal(new Vector3f((float)normal.m_x, (float)normal.m_y, (float)normal.m_z));
    }
    /// <summary>
    /// Append a face normal to the list of mesh face normals.
    /// </summary>
    /// <param name="normal">New face normal.</param>
    /// <returns>The index of the newly added face normal.</returns>
    public int AddFaceNormal(Vector3f normal)
    {
      SetFaceNormal(Count, normal);
      return Count - 1;
    }

    public bool SetFaceNormal(int index, float x, float y, float z)
    {
      return SetFaceNormal(index, new Vector3f(x, y, z));
    }
    public bool SetFaceNormal(int index, double x, double y, double z)
    {
      return SetFaceNormal(index, new Vector3f((float)x, (float)y, (float)z));
    }
    public bool SetFaceNormal(int index, Vector3d normal)
    {
      return SetFaceNormal(index, new Vector3f((float)normal.m_x, (float)normal.m_y, (float)normal.m_z));
    }
    public bool SetFaceNormal(int index, Vector3f normal)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetNormal(ptr, index, normal, true);
    }
    #endregion

    /// <summary>
    /// Unitize all the existing face normals.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool UnitizeFaceNormals()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxUnitizeFaceNormals);
    }

    /// <summary>
    /// Compute all the face normals for this mesh based on the physical shape of the mesh.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool ComputeFaceNormals()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxComputeFaceNormals);
    }
    #endregion
    /*
    #region IEnumerable implementation
    IEnumerator<Vector3f> IEnumerable<Vector3f>.GetEnumerator()
    {
      return new MFEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MFEnum(this);
    }

    private class MFEnum : IEnumerator<Vector3f>
    {
      #region members
      private MeshFaceNormals m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MFEnum(MeshFaceNormals mesh_facenormals)
      {
        m_owner = mesh_facenormals;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Vector3f Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed) { return; }
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion
    */
  }

  /// <summary>
  /// Provides access to the vertex colors of a mesh object.
  /// </summary>
  public class MeshVertexColorList //: IEnumerable<Color>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshVertexColorList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of mesh colors.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxColorCount);
      }
      set
      {
        if (value >= 0 && Count != value)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxColorCount, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the vertex color at the given index. 
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of vertex control to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The vertex color at [index].</returns>
    public Color this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        int argb = 0;
        IntPtr ptr = m_mesh.ConstPointer();
        UnsafeNativeMethods.ON_Mesh_GetColor(ptr, index, ref argb);

        return Color.FromArgb(argb);
      }
      set
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetColor(ptr, index, value.ToArgb());
      }
    }
    #endregion

    #region access
    /// <summary>
    /// Clear the Vertex Color list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearColors);
    }

    /// <summary>
    /// Adds a new vertex color to the end of the Color list.
    /// </summary>
    /// <param name="red">Red component of color, must be in the 0~255 range.</param>
    /// <param name="green">Green component of color, must be in the 0~255 range.</param>
    /// <param name="blue">Blue component of color, must be in the 0~255 range.</param>
    /// <returns>The index of the newly added color.</returns>
    public int Add(int red, int green, int blue)
    {
      SetColor(Count, red, green, blue);
      return Count - 1;
    }
    /// <summary>
    /// Adds a new vertex color to the end of the Color list.
    /// </summary>
    /// <param name="color">Color to append, Alpha channels will be ignored.</param>
    /// <returns>The index of the newly added color.</returns>
    public int Add(Color color)
    {
      SetColor(Count, color);
      return Count - 1;
    }

    /// <summary>
    /// Sets or adds a vertex color to the Color List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex color to set. 
    /// If index equals Count, then the color will be appended.</param>
    /// <param name="red">Red component of vertex color. Value must be in the 0~255 range.</param>
    /// <param name="green">Green component of vertex color. Value must be in the 0~255 range.</param>
    /// <param name="blue">Blue component of vertex color. Value must be in the 0~255 range.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetColor(int index, int red, int green, int blue)
    {
      return SetColor(index, Color.FromArgb(red, green, blue));
    }
    /// <summary>
    /// Sets or adds a vertex to the Vertex List.
    /// <para>If [index] is less than [Count], the existing vertex at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new vertex is appended to the end of the vertex list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of vertex color to set. 
    /// If index equals Count, then the color will be appended.</param>
    /// <param name="color">Color to set, Alpha channels will be ignored.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetColor(int index, Color color)
    {
      if (index < 0 || index > Count)
      {
        throw new IndexOutOfRangeException();
      }

      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetColor(ptr, index, color.ToArgb());
    }
    #endregion

    #region methods
    private bool SetColorsHelper(Color[] colors, bool append)
    {
      if (colors == null) { return false; }

      IntPtr pThis = m_mesh.NonConstPointer();

      int count = colors.Length;
      int[] argb = new int[count];

      for (int i = 0; i < count; i++)
      { argb[i] = colors[i].ToArgb(); }

      return UnsafeNativeMethods.ON_Mesh_SetVertexColors(pThis, count, argb, append);
    }

    /// <summary>
    /// Create a valid vertex color list consisting of a single color.
    /// </summary>
    /// <param name="baseColor">Color to apply to every vertex.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool CreateMonotoneMesh(Color baseColor)
    {
      int count = m_mesh.Vertices.Count;
      Color[] colors = new Color[count];

      for (int i = 0; i < count; i++)
      { colors[i] = baseColor; }

      return SetColors(colors);
    }

    /// <summary>
    /// Set all the vertex colors in one go. For the Mesh to be valid, the number 
    /// of colors must match the number of vertices.
    /// </summary>
    /// <param name="colors">Colors to set.</param>
    /// <returns>True on success, False on failure.</returns>
    public bool SetColors(Color[] colors)
    {
      return SetColorsHelper(colors, false);
    }

    /// <summary>
    /// Append a collection of colors to the Vertex Color list. 
    /// For the Mesh to be valid, the number of colors must match the number of vertices.
    /// </summary>
    /// <param name="colors">Colors to append.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool AppendColors(Color[] colors)
    {
      return SetColorsHelper(colors, true);
    }
    #endregion

    /*
    #region IEnumerable implementation
    IEnumerator<Color> IEnumerable<Color>.GetEnumerator()
    {
      return new MCEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MCEnum(this);
    }

    private class MCEnum : IEnumerator<Color>
    {
      #region members
      private MeshColors m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MCEnum(MeshColors mesh_colors)
      {
        m_owner = mesh_colors;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Color Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed) { return; }
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion
    */
  }

  /// <summary>
  /// Provides access to the Vertex Texture coordinates of a Mesh.
  /// </summary>
  public class MeshTextureCoordinateList // : IEnumerable<Point2f>
  {
    private Mesh m_mesh;

    #region constructors
    internal MeshTextureCoordinateList(Mesh ownerMesh)
    {
      m_mesh = ownerMesh;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the number of texture coordinates.
    /// </summary>
    public int Count
    {
      get
      {
        IntPtr ptr = m_mesh.ConstPointer();
        return UnsafeNativeMethods.ON_Mesh_GetInt(ptr, Mesh.idxTextureCoordinateCount);
      }
      set
      {
        if (value >= 0 && Count != value)
        {
          IntPtr pMesh = m_mesh.NonConstPointer();
          UnsafeNativeMethods.ON_Mesh_SetInt(pMesh, Mesh.idxTextureCoordinateCount, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the texture coordinate at the given index. 
    /// The index must be valid or an IndexOutOfRangeException will be thrown.
    /// </summary>
    /// <param name="index">Index of texture coordinates to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is invalid.</exception>
    /// <returns>The texture coordinate at [index].</returns>
    public Point2f this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        IntPtr ptr = m_mesh.ConstPointer();
        float s = 0;
        float t = 0;
        if (!UnsafeNativeMethods.ON_Mesh_GetTextureCoordinate(ptr, index, ref s, ref t)) { return Point2f.Unset; }
        return new Point2f(s, t);
      }
      set
      {
        if (index < 0 || index >= Count)
          throw new IndexOutOfRangeException();

        IntPtr ptr = m_mesh.NonConstPointer();
        UnsafeNativeMethods.ON_Mesh_SetTextureCoordinate(ptr, index, value.m_x, value.m_y);
      }
    }
    #endregion

    #region access
    /// <summary>
    /// Clear the Texture Coordinate list on the mesh.
    /// </summary>
    public void Clear()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      UnsafeNativeMethods.ON_Mesh_ClearList(ptr, Mesh.idxClearTextureCoordinates);
    }

    /// <summary>
    /// Adds a new texture coordinate to the end of the Texture list.
    /// </summary>
    /// <param name="s">S component of new texture coordinate.</param>
    /// <param name="t">T component of new texture coordinate.</param>
    /// <returns>The index of the newly added texture coordinate.</returns>
    public int Add(float s, float t)
    {
      int N = Count;
      if (!SetTextureCoordinate(N, new Point2f(s, t))) { return -1; }
      return N;
    }
    /// <summary>
    /// Adds a new texture coordinate to the end of the Texture list.
    /// </summary>
    /// <param name="s">S component of new texture coordinate.</param>
    /// <param name="t">T component of new texture coordinate.</param>
    /// <returns>The index of the newly added texture coordinate.</returns>
    public int Add(double s, double t)
    {
      return Add((float)s, (float)t);
    }
    /// <summary>
    /// Adds a new texture coordinate to the end of the Texture list.
    /// </summary>
    /// <param name="tc">Texture coordinate to add.</param>
    /// <returns>The index of the newly added texture coordinate.</returns>
    public int Add(Point2f tc)
    {
      return Add(tc.m_x, tc.m_y);
    }
    /// <summary>
    /// Adds a new texture coordinate to the end of the Texture list.
    /// </summary>
    /// <param name="tc">Texture coordinate to add.</param>
    /// <returns>The index of the newly added texture coordinate.</returns>
    public int Add(Point3d tc)
    {
      return Add((float)tc.m_x, (float)tc.m_y);
    }
    /// <summary>
    /// Append an array of texture coordinates.
    /// </summary>
    /// <param name="textureCoordinates">Texture coordinates to append.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool AddRange(Point2f[] textureCoordinates)
    {
      return SetTextureCoordinatesHelper(textureCoordinates, true);
    }

    /// <summary>
    /// Sets or adds a texture coordinate to the Texture Coordinate List.
    /// <para>If [index] is less than [Count], the existing coordinate at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new coordinate is appended to the end of the coordinate list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of texture coordinate to set.</param>
    /// <param name="s">S component of texture coordinate.</param>
    /// <param name="t">T component of texture coordinate.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetTextureCoordinate(int index, float s, float t)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_SetTextureCoordinate(ptr, index, s, t);
    }
    /// <summary>
    /// Sets or adds a texture coordinate to the Texture Coordinate List.
    /// <para>If [index] is less than [Count], the existing coordinate at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new coordinate is appended to the end of the coordinate list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of texture coordinate to set.</param>
    /// <param name="s">S component of texture coordinate.</param>
    /// <param name="t">T component of texture coordinate.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetTextureCoordinate(int index, double s, double t)
    {
      return SetTextureCoordinate(index, (float)s, (float)t);
    }
    /// <summary>
    /// Sets or adds a texture coordinate to the Texture Coordinate List.
    /// <para>If [index] is less than [Count], the existing coordinate at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new coordinate is appended to the end of the coordinate list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of texture coordinate to set.</param>
    /// <param name="tc">Texture coordinate point.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetTextureCoordinate(int index, Point2f tc)
    {
      return SetTextureCoordinate(index, tc.m_x, tc.m_y);
    }
    /// <summary>
    /// Sets or adds a texture coordinate to the Texture Coordinate List.
    /// <para>If [index] is less than [Count], the existing coordinate at [index] will be modified.</para>
    /// <para>If [index] equals [Count], a new coordinate is appended to the end of the coordinate list.</para> 
    /// <para>If [index] is larger than [Count], the function will return false.</para>
    /// </summary>
    /// <param name="index">Index of texture coordinate to set.</param>
    /// <param name="tc">Texture coordinate point.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetTextureCoordinate(int index, Point3f tc)
    {
      return SetTextureCoordinate(index, (float)tc.m_x, (float)tc.m_y);
    }
    /// <summary>
    /// Set all texture coordinates in one go.
    /// </summary>
    /// <param name="textureCoordinates">Texture coordinates to assign to the mesh.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool SetTextureCoordinates(Point2f[] textureCoordinates)
    {
      return SetTextureCoordinatesHelper(textureCoordinates, false);
    }

    private bool SetTextureCoordinatesHelper(Point2f[] textureCoordinates, bool append)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      if (textureCoordinates == null)
        return false;
      return UnsafeNativeMethods.ON_Mesh_SetTextureCoordinates(ptr, textureCoordinates.Length, ref textureCoordinates[0], append);
    }
    #endregion

    #region methods
    /// <summary>
    /// Scale the texture coordinates so the texture domains are [0,1] 
    /// and eliminate any texture rotations.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool NormalizeTextureCoordinates()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxNormalizeTextureCoordinates);
    }
    /// <summary>
    /// David: What does this do?
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool TransposeTextureCoordinates()
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_NonConstBoolOp(ptr, Mesh.idxTransposeTextureCoordinates);
    }
    /// <summary>
    /// Reverse one coordinate direction of the texture coordinates, within texture domain m_tex_domain
    /// </summary>
    /// <param name="direction">
    /// 0 = first texture coordinate is reversed
    /// 1 = second texture coordinate is reversed
    /// </param>
    /// <returns></returns>
    public bool ReverseTextureCoordinates(int direction)
    {
      IntPtr ptr = m_mesh.NonConstPointer();
      return UnsafeNativeMethods.ON_Mesh_Reverse(ptr, true, direction);
    }
    #endregion

    /*
    #region IEnumerable implementation
    IEnumerator<Point2f> IEnumerable<Point2f>.GetEnumerator()
    {
      return new MTCEnum(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new MTCEnum(this);
    }

    private class MTCEnum : IEnumerator<Point2f>
    {
      #region members
      private MeshTextureCoords m_owner;
      int position = -1;
      #endregion

      #region constructor
      public MTCEnum(MeshTextureCoords mesh_texturecoordinates)
      {
        m_owner = mesh_texturecoordinates;
      }
      #endregion

      #region enumeration logic
      public bool MoveNext()
      {
        position++;
        return (position < m_owner.Count);
      }
      public void Reset()
      {
        position = -1;
      }

      public Point2f Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      object IEnumerator.Current
      {
        get
        {
          try
          {
            return m_owner[position];
          }
          catch (IndexOutOfRangeException)
          {
            throw new InvalidOperationException();
          }
        }
      }
      #endregion

      #region IDisposable logic
      private bool m_disposed; // = false; <- initialized by runtime
      public void Dispose()
      {
        if (m_disposed) { return; }
        m_disposed = true;
        GC.SuppressFinalize(this);
      }
      #endregion
    }
    #endregion
    */
  }
}
//class ON_CLASS ON_MeshVertexRef : public ON_Geometry
//{
//  ON_OBJECT_DECLARE(ON_MeshVertexRef);
//public:
//  ON_MeshVertexRef();
//  ~ON_MeshVertexRef();
//  ON_MeshVertexRef& operator=(const ON_MeshVertexRef&);


//  // parent mesh
//  const ON_Mesh* m_mesh;

//  // m_mesh->m_V[] index
//  // (can be -1 when m_top_vi references a shared vertex location)
//  int m_mesh_vi; 

//  // m_mesh->m_top.m_tope[] index
//  int m_top_vi; 


//  /*
//  Description:
//    Override of the virtual ON_Geometry::ComponentIndex().
//  Returns:
//    A component index for the vertex.  The type of the returned
//    component index can be 
//    ON_ComponentIndex::mesh_vertex, 
//    ON_ComponentIndex::meshtop_vertex, or
//    ON_ComponentIndex::invalid_type.
//  */
//  ON_ComponentIndex ComponentIndex() const;

//  /*
//  Returns:
//    The mesh topology associated with this 
//    mesh vertex reference or NULL if it doesn't
//    exist.
//  */
//  const ON_MeshTopology* MeshTopology() const;

//  /*
//  Returns:
//    The 3d location of the mesh vertex.  Returns
//    ON_UNSET_POINT is this ON_MeshVertexRef is not 
//    valid.
//  */
//  ON_3dPoint Point() const;

//  /*
//  Returns:
//    The mesh topology vertex associated with this 
//    mesh vertex reference.
//  */
//  const ON_MeshTopologyVertex* MeshTopologyVertex() const;

//  // overrides of virtual ON_Object functions
//  BOOL IsValid( ON_TextLog* text_log = NULL ) const;
//  void Dump( ON_TextLog& ) const;
//  unsigned int SizeOf() const;
//  ON::ObjectType ObjectType() const;

//  // overrides of virtual ON_Geometry functions
//  int Dimension() const;
//  BOOL GetBBox(
//         double* boxmin,
//         double* boxmax,
//         int bGrowBox = false
//         ) const;
//  BOOL Transform( 
//         const ON_Xform& xform
//         );
//};

//class ON_CLASS ON_MeshEdgeRef : public ON_Geometry
//{
//  ON_OBJECT_DECLARE(ON_MeshEdgeRef);
//public:
//  ON_MeshEdgeRef();
//  ~ON_MeshEdgeRef();
//  ON_MeshEdgeRef& operator=(const ON_MeshEdgeRef&);

//  // parent mesh
//  const ON_Mesh* m_mesh;

//  // m_mesh->m_top.m_tope[] index
//  int m_top_ei; 

//  /*
//  Description:
//    Override of the virtual ON_Geometry::ComponentIndex().
//  Returns:
//    A mesh component index for the edge.  The type is
//    ON_ComponentIndex::meshtop_edge and the index is the
//    index into the ON_MeshTopology.m_tope[] array.
//  */
//  ON_ComponentIndex ComponentIndex() const;

//  /*
//  Returns:
//    The mesh topology associated with this 
//    mesh edge reference or NULL if it doesn't
//    exist.
//  */

//  const ON_MeshTopology* MeshTopology() const;
//  /*
//  Returns:
//    The 3d location of the mesh edge.  Returns
//    ON_UNSET_POINT,ON_UNSET_POINT, is this ON_MeshEdgeRef
//    is not valid.
//  */
//  ON_Line Line() const;

//  /*
//  Returns:
//    The mesh topology edge associated with this 
//    mesh edge reference.
//  */
//  const ON_MeshTopologyEdge* MeshTopologyEdge() const;

//  // overrides of virtual ON_Object functions
//  BOOL IsValid( ON_TextLog* text_log = NULL ) const;
//  void Dump( ON_TextLog& ) const;
//  unsigned int SizeOf() const;
//  ON::ObjectType ObjectType() const;

//  // overrides of virtual ON_Geometry functions
//  int Dimension() const;
//  BOOL GetBBox(
//         double* boxmin,
//         double* boxmax,
//         int bGrowBox = false
//         ) const;
//  BOOL Transform( 
//         const ON_Xform& xform
//         );
//};

//class ON_CLASS ON_MeshFaceRef : public ON_Geometry
//{
//  ON_OBJECT_DECLARE(ON_MeshFaceRef);
//public:
//  ON_MeshFaceRef();
//  ~ON_MeshFaceRef();
//  ON_MeshFaceRef& operator=(const ON_MeshFaceRef&);

//  // parent mesh
//  const ON_Mesh* m_mesh;

//  // m_mesh->m_F[] and m_mesh->m_top.m_tope[] index.
//  int m_mesh_fi; 

//  /*
//  Description:
//    Override of the virtual ON_Geometry::ComponentIndex().
//  Returns:
//    A mesh component index for the face.  The type is
//    ON_ComponentIndex::mesh_face and the index is the
//    index into the ON_Mesh.m_F[] array.
//  */
//  ON_ComponentIndex ComponentIndex() const;

//  /*
//  Returns:
//    The mesh topology associated with this 
//    mesh face reference or NULL if it doesn't
//    exist.
//  */
//  const ON_MeshTopology* MeshTopology() const;

//  /*
//  Returns:
//    The mesh face associated with this mesh face reference.
//  */
//  const ON_MeshFace* MeshFace() const;

//  /*
//  Returns:
//    The mesh topology face associated with this 
//    mesh face reference.
//  */
//  const ON_MeshTopologyFace* MeshTopologyFace() const;

//  // overrides of virtual ON_Object functions
//  BOOL IsValid( ON_TextLog* text_log = NULL ) const;
//  void Dump( ON_TextLog& ) const;
//  unsigned int SizeOf() const;
//  ON::ObjectType ObjectType() const;

//  // overrides of virtual ON_Geometry functions
//  int Dimension() const;
//  BOOL GetBBox(
//         double* boxmin,
//         double* boxmax,
//         int bGrowBox = false
//         ) const;
//  BOOL Transform( 
//         const ON_Xform& xform
//         );
//};

//*
//Description:
//  Calculate a quick and dirty polygon mesh approximation
//  of a surface.
//Parameters:
//  surface - [in]
//  mesh_density - [in] If <= 10, this number controls
//        the relative polygon count.  If > 10, this number
//        specifies a target number of polygons.
//  mesh - [in] if not NULL, the polygon mesh will be put
//              on this mesh.
//Returns:
//  A polygon mesh approximation of the surface or NULL
//  if the surface could not be meshed.
//*/
//ON_DECL
//ON_Mesh* ON_MeshSurface( 
//            const ON_Surface& surface, 
//            int mesh_density = 0,
//            ON_Mesh* mesh = 0
//            );

//*
//Description:
//  Calculate a quick and dirty polygon mesh approximation
//  of a surface.
//Parameters:
//  surface - [in]
//  u_count - [in] >= 2 Number of "u" parameters in u[] array.
//  u       - [in] u parameters
//  v_count - [in] >= 2 Number of "v" parameters in v[] array.
//  v       - [in] v parameters
//  mesh - [in] if not NULL, the polygon mesh will be put
//              on this mesh.
//Returns:
//  A polygon mesh approximation of the surface or NULL
//  if the surface could not be meshed.
//*/
//ON_DECL
//ON_Mesh* ON_MeshSurface( 
//            const ON_Surface& surface, 
//            int u_count,
//            const double* u,
//            int v_count,
//            const double* v,
//            ON_Mesh* mesh = 0
//            );

//*
//Description:
//  Finds the barycentric coordinates of the point on a 
//  triangle that is closest to P.
//Parameters:
//  A - [in] triangle corner
//  B - [in] triangle corner
//  C - [in] triangle corner
//  P - [in] point to test
//  a - [out] barycentric coordinate
//  b - [out] barycentric coordinate
//  c - [out] barycentric coordinate
//        If ON_ClosestPointToTriangle() returns true, then
//        (*a)*A + (*b)*B + (*c)*C is the point on the 
//        triangle's plane that is closest to P.  It is 
//        always the case that *a + *b + *c = 1, but this
//        function will return negative barycentric 
//        coordinate if the point on the plane is not
//        inside the triangle.
//Returns:
//  True if the triangle is not degenerate.  False if the
//  triangle is degenerate; in this case the returned
//  closest point is the input point that is closest to P.
//*/
//ON_DECL
//bool ON_ClosestPointToTriangle( 
//        ON_3dPoint A, ON_3dPoint B, ON_3dPoint C,
//        ON_3dPoint P,
//        double* a, double* b, double* c
//        );


//*
//Description:
//  Finds the barycentric coordinates of the point on a 
//  triangle that is closest to P.
//Parameters:
//  A - [in] triangle corner
//  B - [in] triangle corner
//  C - [in] triangle corner
//  P - [in] point to test
//  a - [out] barycentric coordinate
//  b - [out] barycentric coordinate
//  c - [out] barycentric coordinate
//        If ON_ClosestPointToTriangle() returns true, then
//        (*a)*A + (*b)*B + (*c)*C is the point on the 
//        triangle's plane that is closest to P.  It is 
//        always the case that *a + *b + *c = 1, but this
//        function will return negative barycentric 
//        coordinate if the point on the plane is not
//        inside the triangle.
//Returns:
//  True if the triangle is not degenerate.  False if the
//  triangle is degenerate; in this case the returned
//  closest point is the input point that is closest to P.
//*/
//ON_DECL
//bool ON_ClosestPointToTriangleFast( 
//          const ON_3dPoint& A, 
//          const ON_3dPoint& B, 
//          const ON_3dPoint& C, 
//          ON_3dPoint P,
//          double* a, double* b, double* c
//          );


//*
//Description:
//  Calculate a mesh representation of the NURBS surface's control polygon.
//Parameters:
//  nurbs_surface - [in]
//  bCleanMesh - [in] If true, then degenerate quads are cleaned
//                    up to be triangles. Surfaces with singular
//                    sides are a common source of degenerate qauds.
//  input_mesh - [in] If NULL, then the returned mesh is created
//       by a class to new ON_Mesh().  If not null, then this 
//       mesh will be used to store the conrol polygon.
//Returns:
//  If successful, a pointer to a mesh.
//*/
//ON_DECL
//ON_Mesh* ON_ControlPolygonMesh( 
//          const ON_NurbsSurface& nurbs_surface, 
//          bool bCleanMesh,
//          ON_Mesh* input_mesh = NULL
//          );

//*
//Description:
//  Finds the intersection between a line segment an a triangle.
//Parameters:
//  A - [in] triangle corner
//  B - [in] triangle corner
//  C - [in] triangle corner
//  P - [in] start of line segment
//  Q - [in] end of line segment
//  abc - [out] 
//     barycentric coordinates of intersection point(s)
//  t - [out] line coordinate of intersection point(s)
//Returns:
//  0 - no intersection
//  1 - one intersection point
//  2 - intersection segment
//*/
//ON_DECL
//int ON_LineTriangleIntersect(
//        const ON_3dPoint& A,
//        const ON_3dPoint& B,
//        const ON_3dPoint& C,
//        const ON_3dPoint& P,
//        const ON_3dPoint& Q,
//        double abc[2][3], 
//        double t[2],
//        double tol
//        );

//*
//Description:
//  Finds the unit normal to the triangle
//Parameters:
//  A - [in] triangle corner
//  B - [in] triangle corner
//  C - [in] triangle corner
//Returns:
//  Unit normal
//*/
//ON_DECL
//ON_3dVector ON_TriangleNormal(
//        const ON_3dPoint& A,
//        const ON_3dPoint& B,
//        const ON_3dPoint& C
//        );

//*
//Description:
//  Triangulate a 2D simple closed polygon.
//Parameters:
//  point_count - [in] number of points in polygon ( >= 3 )
//  point_stride - [in]
//  P - [in] 
//    i-th point = (P[i*point_stride], P[i*point_stride+1])
//  tri_stride - [in]
//  triangle - [out]
//    array of (point_count-2)*tri_stride integers
//Returns:
//  True if successful.  In this case, the polygon is trianglulated into 
//  point_count-2 triangles.  The indexes of the 3 points that are the 
//  corner of the i-th (0<= i < point_count-2) triangle are
//    (triangle[i*tri_stride], triangle[i*tri_stride+1], triangle[i*tri_stride+2]).
//Remarks:
//  Do NOT duplicate the start/end point; i.e., a triangle will have
//  a point count of 3 and P will specify 3 distinct non-collinear points.
//*/
//ON_DECL
//bool ON_Mesh2dPolygon( 
//          int point_count,
//          int point_stride,
//          const double* P,
//          int tri_stride,
//          int* triangle 
//          );

//#endif
namespace Rhino.Geometry
{
  [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
  [DebuggerDisplay("{DebuggerDisplayUtil}")]
  [Serializable()]
  public struct MeshFace
  {
    #region members
    internal int m_a;
    internal int m_b;
    internal int m_c;
    internal int m_d;
    #endregion

    #region constructors
    /// <summary>
    /// Create a new triangular Mesh face.
    /// </summary>
    /// <param name="a">Index of first corner.</param>
    /// <param name="b">Index of second corner.</param>
    /// <param name="c">Index of third corner.</param>
    public MeshFace(int a, int b, int c)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = c;
    }
    /// <summary>
    /// Create a new quadrangular Mesh face.
    /// </summary>
    /// <param name="a">Index of first corner.</param>
    /// <param name="b">Index of second corner.</param>
    /// <param name="c">Index of third corner.</param>
    /// <param name="d">Index of fourth corner.</param>
    public MeshFace(int a, int b, int c, int d)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = d;
    }

    /// <summary>
    /// Gets an Unset MeshFace. Unset faces have Int32.MinValue for all corner indices.
    /// </summary>
    public static MeshFace Unset
    {
      get { return new MeshFace(int.MinValue, int.MinValue, int.MinValue); }
    }
    #endregion

    #region properties
    /// <summary>
    /// Internal property that figures out the debugger display for Mesh Faces.
    /// </summary>
    internal string DebuggerDisplayUtil
    {
      get
      {
        if (IsTriangle)
        {
          return string.Format(System.Globalization.CultureInfo.InvariantCulture, "T({0}, {1}, {2})", m_a, m_b, m_c);
        }
        else
        {
          return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Q({0}, {1}, {2}, {3})", m_a, m_b, m_c, m_d);
        }
      }
    }

    /// <summary>
    /// Gets or sets the first corner index of the mesh face.
    /// </summary>
    public int A
    {
      get { return m_a; }
      set { m_a = value; }
    }
    /// <summary>
    /// Gets or sets the second corner index of the mesh face.
    /// </summary>
    public int B
    {
      get { return m_b; }
      set { m_b = value; }
    }
    /// <summary>
    /// Gets or sets the third corner index of the mesh face.
    /// </summary>
    public int C
    {
      get { return m_c; }
      set { m_c = value; }
    }
    /// <summary>
    /// Gets or sets the fourth corner index of the mesh face. 
    /// If D equals C, the mesh face is considered to be a triangle 
    /// rather than a quad.
    /// </summary>
    public int D
    {
      get { return m_d; }
      set { m_d = value; }
    }

    /// <summary>
    /// Gets a value indicating whether or not this mesh face 
    /// is considered to be valid. Note that even valid mesh faces 
    /// could potentially be invalid in the context of a specific Mesh, 
    /// if one or more of the corner indices exceeds the number of 
    /// vertices on the mesh. If you want to perform a complete 
    /// validity check, use IsValid(int) instead.
    /// </summary>
    public bool IsValid()
    {
      if (m_a < 0) { return false; }
      if (m_b < 0) { return false; }
      if (m_c < 0) { return false; }
      if (m_d < 0) { return false; }

      if (m_a == m_b) { return false; }
      if (m_a == m_c) { return false; }
      if (m_a == m_d) { return false; }
      if (m_b == m_c) { return false; }
      if (m_b == m_d) { return false; }

      return true;
    }
    /// <summary>
    /// Gets a value indicating whether or not this mesh face 
    /// is considered to be valid. Unlike the simple IsValid function, 
    /// this function takes upper bound indices into account.
    /// </summary>
    /// <param name="vertexCount">Number of vertices in the mesh that this face is a part of.</param>
    /// <returns>True if the face is considered valid, false if not.</returns>
    public bool IsValid(int vertexCount)
    {
      if (!IsValid()) { return false; }

      if (m_a >= vertexCount) { return false; }
      if (m_b >= vertexCount) { return false; }
      if (m_c >= vertexCount) { return false; }
      if (m_d >= vertexCount) { return false; }

      return true;
    }

    /// <summary>
    /// Gets a value indicating whether or not this mesh face is a triangle. 
    /// A mesh face is considered to be a triangle when C equals D, thus it is 
    /// possible for an Invalid mesh face to also be a triangle.
    /// </summary>
    public bool IsTriangle { get { return m_c == m_d; } }
    /// <summary>
    /// Gets a value indicating whether or not this mesh face is a quad. 
    /// A mesh face is considered to be a triangle when C does not equal D, 
    /// thus it is possible for an Invalid mesh face to also be a quad.
    /// </summary>
    public bool IsQuad { get { return m_c != m_d; } }
    #endregion

    #region methods
    /// <summary>
    /// Set all the corners for this face as a triangle.
    /// </summary>
    /// <param name="a">Index of first corner.</param>
    /// <param name="b">Index of second corner.</param>
    /// <param name="c">Index of third corner.</param>
    public void Set(int a, int b, int c)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = c;
    }
    /// <summary>
    /// Set all the corners for this face as a quad.
    /// </summary>
    /// <param name="a">Index of first corner.</param>
    /// <param name="b">Index of second corner.</param>
    /// <param name="c">Index of third corner.</param>
    /// <param name="d">Index of fourth corner.</param>
    public void Set(int a, int b, int c, int d)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = d;
    }

    /// <summary>
    /// Reverses the orientation of the face by swapping corners. 
    /// The first corner is always maintained.
    /// </summary>
    public MeshFace Flip()
    {
      if (m_c == m_d)
        return new MeshFace(m_a, m_c, m_b, m_b);
      return new MeshFace(m_a, m_d, m_c, m_a);
    }
    #endregion
  }
}
