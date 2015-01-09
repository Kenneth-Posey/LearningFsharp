﻿namespace WpfSimpleDrawingApplication

type Point = { X: float; Y: float }
type PointPair = { Start : Point; End : Point }

type CaptureStatus =
    | Captured
    | Released
type MoveEvent =
    | CaptureChanged of status:CaptureStatus
    | PositionChanged of status:CaptureStatus * position:Point
