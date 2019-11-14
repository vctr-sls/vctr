#!/bin/bash

TARGET=Release
SELF_CONTAINED=true

RIDS=(
  "linux-x64"
  "linux-arm"
  "win-x64"
)

BIN_DIR="bin/${TARGET}/netcoreapp2.1"
PUB_DIR="publish"

ROOT=$PWD

function build {
  RID=$1
  dotnet publish \
    -c $TARGET \
    -r $RID \
    --self-contained $SELF_CONTAINED
}

function pack {
  RID=$1

  cd ${ROOT}/${BIN_DIR}/${RID}/publish
  [ -d ${ROOT}/${PUB_DIR} ] || mkdir -p ${ROOT}/${PUB_DIR}
  tar -czvf ${ROOT}/${PUB_DIR}/vctr-${TARGET}-${RID}.tgz *
  cd $ROOT
}

for RID in "${RIDS[@]}"; do
  echo "--------------------"
  echo "BUILDING $RID..."
  echo "--------------------"

  build $RID
  pack $RID
done

cd ${ROOT}/${PUB_DIR}
sha256sum * >> sha256sums.txt
