package medpointsdb_test

import (
	"bytes"
	"fmt"
	"strconv"
	"sync"
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

func TestLvlDB_MultiThreadTest(t *testing.T) {
	t.Parallel()
	db, clean_up := createDbTestInstance()
	defer clean_up()

	const n = 16
	var pending sync.WaitGroup

	pending.Add(n)
	for i := 0; i < n; i++ {
		go func(key string) {
			defer pending.Done()
			err := db.Put([]byte(key), []byte("v"+key))
			if err != nil {
				panic("put broken: " + err.Error())
			}
		}(strconv.Itoa(i))
	}
	pending.Wait()

	pending.Add(n)
	for i := 0; i < n; i++ {
		go func(key string) {
			defer pending.Done()
			data, err := db.Get([]byte(key))
			if err != nil {
				panic("get failed: " + err.Error())
			}
			if !bytes.Equal(data, []byte("v"+key)) {
				panic(fmt.Sprintf("get brokent, %q != %q", []byte(data), []byte("v"+key)))
			}
		}(strconv.Itoa(i))
	}
	pending.Wait()

	pending.Add(n)
	for i := 0; i < n; i++ {
		go func(key string) {
			defer pending.Done()
			err := db.Delete([]byte(key))
			if err != nil {
				panic("delete failed: " + err.Error())
			}
		}(strconv.Itoa(i))
	}
	pending.Wait()

	pending.Add(n)
	for i := 0; i < n; i++ {
		go func(key string) {
			defer pending.Done()
			_, err := db.Get([]byte(key))
			if err == nil {
				panic("get find something")
			}
		}(strconv.Itoa(i))
	}
	pending.Wait()
}
