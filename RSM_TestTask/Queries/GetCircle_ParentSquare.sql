Select obj1.Id, obj1.ObjectType, dbo.CircleArea(Changes.OldMainSize) as Area
From Objects as obj1 
Join Objects as obj2 on obj2.id = obj1.ParentId 
Join Changes on Changes.ChangableRow = obj1.Id 
Where obj1.ObjectType=0 and obj2.ObjectType = 1 and ChangeDate>=@minDate and ChangeDate<=@maxDate 