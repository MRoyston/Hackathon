﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;
    private static List<GameObject> selectedPath = new List<GameObject>();
	private SpriteRenderer render;
	private bool isSelected = false;
    private static bool mousedown;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Vector2[] adjacentDirectionsVert = new Vector2[] { Vector2.up, Vector2.zero, Vector2.down};
    private Vector2[] adjacentDirectionsHorz = new Vector2[] { Vector2.left, Vector2.zero, Vector2.right};



    void Awake() {
		render = GetComponent<SpriteRenderer>();
    }
    private void StartPath()
    {
        selectedPath.Clear();
        selectedPath.Add(gameObject);
        Select();
    }
	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}
    private void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.instance.IsShifting)
        {
            return;
        }
        mousedown = true;
        StartPath();
    }
    private void OnMouseEnter()
    {
        if(mousedown)
        {
            if (isAdjacent() && previousSelected.render.sprite == render.sprite)
            {
                selectedPath.Add(gameObject);
                Select();
            }
        }
    }
    private void OnMouseUp()
    {
        mousedown = false;
        for(int i=selectedPath.Count-1; i>=0;i--)
        {
            selectedPath[i].GetComponent<Tile>().Deselect();
            selectedPath.Remove(selectedPath[i]);
        }
    }
    private bool isAdjacent()
    {
        GameObject tempVert;
        GameObject temp;
        for(int i =0; i<adjacentDirectionsVert.Length;i++)
        {
            if(adjacentDirectionsVert[i]!=Vector2.zero)
            {
                tempVert = GetAdjacent(adjacentDirectionsVert[i]);
            }
            else
            {
                tempVert = gameObject;
            }
            for(int j=0;j<adjacentDirectionsHorz.Length;j++)
            {
                if(adjacentDirectionsHorz[j]!=Vector2.zero&&tempVert)
                {
                    temp = tempVert.GetComponent<Tile>().GetAdjacent(adjacentDirectionsHorz[j]);
                }
                else
                {
                    temp = tempVert;
                }
                if(previousSelected.gameObject == temp)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

}