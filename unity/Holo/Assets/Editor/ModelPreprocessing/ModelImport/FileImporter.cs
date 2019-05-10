﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//A class for loading a specific file. Currently STL and specific VTK formats are supported.
public class FileImporter
{
    private STLImporter sTLImporter;
    private VTKImporter vtkImporter;
    private string fileExtension;

    public Vector3[] BaseVertices { get; private set; }
    public Vector3[] Vertices { get; private set; }
    public Vector3[] Normals { get; private set; }
    public int[] Indices { get; private set; }
    public int IndicesInFacet { get; private set; }
    public Dictionary<string, Vector3> BoundingVertices { get; private set; } = new Dictionary<string, Vector3>()
    { { "minVertex", new Vector3()},
      { "maxVertex", new Vector3()}
    };

    //A constructor ensuring FileImporter is format-specific (only STL and VTK for now)
    public FileImporter(string extension)
    {
        fileExtension = extension;
        if (extension == ".stl")
            sTLImporter = new STLImporter();
        else if (extension == ".vtk")
        {
            vtkImporter = new VTKImporter();
        }
        else
        {
            EditorUtility.ClearProgressBar();
            throw new Exception("Type not supported!");
        }
    }

    //Loads a mesh from the given filepath.
    public void LoadFile(string filePath, bool firstMesh)
    {

        switch (fileExtension)
        {
            case ".stl":
                LoadStlFile(filePath);
                break;
            case ".vtk":
                LoadVtkFile(filePath);
                break;
        }
        if (firstMesh)
            BaseVertices = new Vector3[Vertices.Length];
    }

    //Loads a mesh from the STL file located in the given filepath.
    private void LoadStlFile(string filePath)
    {
        sTLImporter.LoadFile(filePath);
        Vertices = sTLImporter.Vertices;
        Indices = sTLImporter.Indices;
        Normals = sTLImporter.Normals;
    }

    //Loads a mesh from the VTK file located in the given filepath.
    private void LoadVtkFile(string filePath)
    {
        vtkImporter.LoadFile(filePath);
        Vertices = vtkImporter.Vertices;
        Indices = vtkImporter.Indices;
        IndicesInFacet = vtkImporter.IndicesInFacet;
        Normals = vtkImporter.Normals;
        BoundingVertices = vtkImporter.BoundingVertices;
    }
}
