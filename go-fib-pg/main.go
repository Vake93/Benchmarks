package main

import (
	"database/sql"
	"github.com/gofiber/fiber/v2"
	_ "github.com/lib/pq"
)

type Fortune struct {
	Id      int    `json:"id"`
	Message string `json:"message"`
}

type World struct {
	Id      int    `json:"id"`
	RandomNumber int `json:"randomNumber"`
}

func main() {

	db, _ := sql.Open("postgres", "postgresql://user:user@123@localhost:5432/fortunes?sslmode=disable")
	db.SetMaxOpenConns(100)
	db.SetMaxIdleConns(100)

	app := fiber.New()

	app.Get("/text", func(c *fiber.Ctx) error {
		return c.SendString("Hello World")
	})

	app.Get("/json", func(c *fiber.Ctx) error {
		return c.JSON(Fortune{
			Id: 1,
			Message: "Hello World",
		})
	})

	app.Get("/single", func(c *fiber.Ctx) error {

		var fortunes []Fortune
		rows, _ := db.Query("SELECT id, message FROM fortune ORDER BY id")

		for rows.Next() {
			var fortune = Fortune{}
			rows.Scan(&fortune.Id, &fortune.Message)
			fortunes = append(fortunes, fortune)
		}

		return c.JSON(fortunes)
	})

	app.Get("/multiple", func(c *fiber.Ctx) error {

		row := db.QueryRow("SELECT id, randomnumber FROM world WHERE id = 260")

		var item World
		row.Scan(&item.Id, &item.RandomNumber)

		var results []World
		rows, _ := db.Query("SELECT id, randomnumber FROM world WHERE randomnumber > $1 ORDER BY id", item.RandomNumber)

		for rows.Next() {
			var world = World{}
			rows.Scan(&world.Id, &world.RandomNumber)
			results = append(results, world)
		}

		return c.JSON(results)
	})

	app.Listen(":5000")
}
