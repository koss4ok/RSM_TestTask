Select objects.Id,ObjectType, CASE 
	WHEN ObjectType=0 THEN dbo.CircleArea(Changes.OldMainSize)
	WHEN ObjectType=1 THEN dbo.SquareArea(Changes.OldMainSize)
	WHEN ObjectType=2 THEN dbo.RectArea(Changes.OldMainSize,Changes.OldAddictSize)
END AS Area 
From Objects 
Join Changes on changes.ChangableRow = objects.Id
Where ChangeDate>=@minDate and ChangeDate<=@maxDate
Order By ParentId