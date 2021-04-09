RESULT=$(dotnet list package --outdated)

COUNT=$(echo "$RESULT" | grep "^   > " | wc -l)

if [ $COUNT -gt 0 ]
then
	echo "$RESULT"
	exit 1
else
	exit 0
fi
