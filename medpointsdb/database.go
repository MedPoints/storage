package medpointsdb

import (
	"log"
	"os"

	"github.com/syndtr/goleveldb/leveldb"
	"github.com/syndtr/goleveldb/leveldb/errors"
	"github.com/syndtr/goleveldb/leveldb/filter"
	"github.com/syndtr/goleveldb/leveldb/opt"
)

func newLog(logpath string) *log.Logger {
	println("LogFile: " + logpath)
	file, err := os.Create(logpath)
	if err != nil {
		panic(err)
	}
	return log.New(file, "", log.LstdFlags|log.Lshortfile)
}

type LvlDBDatabase struct {
	db   *leveldb.DB // LevelDB
	path string
	log  *log.Logger
}

func NewLvlDBDatabase(file string, cache int, handles int) (*LvlDBDatabase, error) {
	log := newLog("database")

	log.Printf("Database file name %v", file)

	db, err := leveldb.OpenFile(file, &opt.Options{
		OpenFilesCacheCapacity: handles,
		BlockCacheCapacity:     cache / 2 * opt.MiB,
		WriteBuffer:            cache / 4 * opt.MiB,
		Filter:                 filter.NewBloomFilter(10),
	})
	if _, corrupted := err.(*errors.ErrCorrupted); corrupted {
		db, err = leveldb.RecoverFile(file, nil)
	}
	if err != nil {
		return nil, err
	}
	return &LvlDBDatabase{
		path: file,
		db:   db,
		log:  log,
	}, nil
}

func (db *LvlDBDatabase) PathToDatabase() string {
	return db.path
}

func (db *LvlDBDatabase) LvlDB() *leveldb.DB {
	return db.db
}

func (db *LvlDBDatabase) Put(key []byte, value []byte) error {
	return db.db.Put(key, value, nil)
}

func (db *LvlDBDatabase) Has(key []byte) (bool, error) {
	return db.db.Has(key, nil)
}

func (db *LvlDBDatabase) Get(key []byte) ([]byte, error) {
	dat, err := db.db.Get(key, nil)
	if err != nil {
		return nil, err
	}
	return dat, nil
}

func (db *LvlDBDatabase) Delete(key []byte) error {
	return db.db.Delete(key, nil)
}

func (db *LvlDBDatabase) Close() {
	err := db.db.Close()
	if err == nil {
		db.log.Printf("Database closed")
	} else {
		db.log.Panicf("Failed to close database %v", err)
	}
}
