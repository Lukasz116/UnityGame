﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour 
{
    // Wybor trybu ruchu. Lerp - wykorzystujacy interpolacje liniowa.
    public enum FollowType
    {
        MoveTowards,
        Lerp
    }

    public FollowType Type = FollowType.MoveTowards;
    public PathDefinition Path;
    public float Speed = 1;
    public float MaxDistanceToGoal = .1f;

    private IEnumerator<Transform> _currentPoint;

    // Inicjalizacja.
    public void Start()
    {
        if (Path == null)
        {
            Debug.LogError("Path can't be null", gameObject);
            return;
        }

        _currentPoint = Path.GetPathEnumerator();
        _currentPoint.MoveNext();

        if (_currentPoint.Current == null)
            return;

        transform.position = _currentPoint.Current.position;
    }

    public void Update()
    {
        // Obiekt stworzony bez sciezki, albo ze sciezka ktora nie ma
        // co najmniej jednego punktu.
        if (_currentPoint == null || _currentPoint.Current == null)
            return;

        // Argumenty: obecna pozycja, pozycja docelowa, predkosc 
        // (skalowana przez deltaTime, aby zapewnic plynnosc animacji).
        if (Type == FollowType.MoveTowards)
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);
        else if (Type == FollowType.Lerp)
            transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);

        // Sprawdzenie czy osiagnelismy cel i mozemy przemiescic sie do kolejnego punktu.
        var distanceSquared = (transform.position - _currentPoint.Current.position).sqrMagnitude;
        if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
            _currentPoint.MoveNext();
    }
}