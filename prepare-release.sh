BIN_PATH="bin/Release"
VERSION=$1

if [ -z "$VERSION" ]; then
	echo please specify version number
	exit
fi

FILENAME="kenja-csharp-parser-${VERSION}.tar.gz"
find $BIN_PATH \( -name '*.dll' -o -name '*.exe' \) -type f -exec basename {} \; | xargs tar cfz $FILENAME -C $BIN_PATH
echo $FILENAME was created.
