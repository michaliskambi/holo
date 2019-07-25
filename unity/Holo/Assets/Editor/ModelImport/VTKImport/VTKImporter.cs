﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace ModelImport.VTKImport
{
    public class VTKImporter
    {
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector3[] DeltaTangents { get; private set; }
        public int[] Indices { get; private set; }
        public int VerticesInFacet { get; private set; }
        public Dictionary<string, Vector3> BoundingVertices { get; private set; } = new Dictionary<string, Vector3>()
        {
            { "minVertex", new Vector3()},
            { "maxVertex", new Vector3()}
        };


        private UnstructuredGridImporter unstructuredGridImporter = new UnstructuredGridImporter();
        private PolyDataImporter polyDataImporter = new PolyDataImporter();

        private bool simulationData = false;

        public VTKImporter(bool IsSimulationData)
        {
            simulationData = IsSimulationData;
        }

        //Loads a mesh from the VTK file located in the given filepath.
        public void ImportFile(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.ASCII))
            {
                streamReader.ReadLine(); //DataFile version
                streamReader.ReadLine(); //vtk output

                string encoding = streamReader.ReadLine();
                if (!encoding.Equals("ASCII"))
                {
                    throw new Exception("Wrong file encoding!");
                }

                string[] datatype = streamReader.ReadLine().Split(' ');

                /* Skip all lines until an empty line.
                 * Allows to ignore lines like
                 * 
                 * FIELD FieldData 1
                 * MaterialNames 1 1 string
                 * None
                 * 
                 * in test UnicornHeart/meshes_body/unicornMesh01.vtk 
                 */
                string headerLine;
                do { 
                    headerLine = streamReader.ReadLine();
                } while (!String.IsNullOrWhiteSpace(headerLine));

                switch (datatype[1])
                {
                    case "POLYDATA":
                        polyDataImporter.LoadFile(streamReader);
                        Indices = polyDataImporter.Indices;
                        Vertices = polyDataImporter.Vertices;
                        Normals = polyDataImporter.Normals;
                        break;
                    case "UNSTRUCTURED_GRID":
                        unstructuredGridImporter.ImportFile(streamReader, simulationData);
                        Indices = unstructuredGridImporter.Indices;
                        Vertices = unstructuredGridImporter.Vertices;
                        Normals = unstructuredGridImporter.Normals;
                        DeltaTangents = unstructuredGridImporter.DeltaTangents;
                        VerticesInFacet = unstructuredGridImporter.VerticesInFacet;
                        BoundingVertices = unstructuredGridImporter.BoundingVertices;

                        break;
                    default:
                        throw new Exception("Wrong file datatype!");
                }
            }
        }
    }
}