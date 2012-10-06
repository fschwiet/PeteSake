SELECT UserId,Username FROM Users FOR XML AUTO
SELECT CollectionId,CollectionTypeId,Name FROM Collection FOR XML AUTO
SELECT * FROM CollectionItem FOR XML AUTO
SELECT * FROM ExternalVotingData FOR XML AUTO
SELECT Name,Url FROM Asset FOR XML AUTO