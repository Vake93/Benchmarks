package main

import (
	"github.com/gin-gonic/gin"
	"github.com/jinzhu/gorm"
	_ "github.com/jinzhu/gorm/dialects/postgres"
	"net/http"
)

var DB *gorm.DB

type Fortune struct {
	Id int `json:"id" gorm:"primary_key"`
	Message string `json:"message"`
}

func (Fortune) TableName() string {
	return "fortune"
}

func main() {
	gin.SetMode(gin.ReleaseMode)

	database, err := gorm.Open("postgres", "postgresql://viq.api:viq.api.123@localhost:5432/fortunes?sslmode=disable")

	if err != nil {
		panic(err.Error())
	}

	DB = database

	app := gin.New()
	app.GET("/", GetFortunes)
	app.Run(":5000")
}

func GetFortunes(c *gin.Context){
	var fortunes []Fortune
	DB.Find(&fortunes)

	c.JSON(http.StatusOK, fortunes)
}