package medpointsdb_test

import (
	"bytes"
	"testing"

	"github.com/MedPoints/storage/medpointsdb"

	"io/ioutil"
	"os"
)

func createDbTestInstance() (*medpointsdb.LvlDBDatabase, func()) {
	dirname, err := ioutil.TempDir(os.TempDir(), "medpointsdb_testdatabase")
	if err != nil {
		panic("problems with creating medpointsdb" + err.Error())
	}
	db, err := medpointsdb.NewLvlDBDatabase(dirname, 0, 0)
	if err != nil {
		panic("problems with creating medpointsdb " + err.Error())
	}

	return db, func() {
		db.Close()
		os.RemoveAll(dirname)
	}
}

func TestLvlDB_GetPutCommonTest(t *testing.T) {
	var values_for_test = []string{"", "z", "1251"}

	t.Parallel()
	db, clean_up := createDbTestInstance()
	defer clean_up()

	for _, v := range values_for_test {
		err := db.Put([]byte(v), []byte(v))
		if err != nil {
			t.Fatalf("put broken: %v", err)
		}
	}

	for _, v := range values_for_test {
		data, err := db.Get([]byte(v))
		if err != nil {
			t.Fatalf("get broken: %v", err)
		}
		if !bytes.Equal(data, []byte(v)) {
			t.Fatalf("broken result, %q != %q", string(data), v)
		}
	}

	for _, v := range values_for_test {
		err := db.Put([]byte(v), []byte("new value"))
		if err != nil {
			t.Fatalf("put rewrite broken: %v", err)
		}
	}

	for _, v := range values_for_test {
		err := db.Delete([]byte(v))
		if err != nil {
			t.Fatalf("problems with deleting %q: %v", v, err)
		}
	}

	for _, v := range values_for_test {
		_, err := db.Get([]byte(v))
		if err == nil {
			t.Fatalf("value should be delete %q", v)
		}
	}
}
