package main

import (
	"database/sql"
	"log"

	"github.com/gofiber/fiber/v2"
	_ "github.com/lib/pq"
)

type Fortune struct {
	Id      int    `json:"id"`
	Message string `json:"message"`
}

func main() {

	db, err := sql.Open("postgres", "postgresql://viq.api:viq.api.123@localhost:5432/fortunes?sslmode=disable")

	if err != nil {
		log.Fatal(err)
	}

	app := fiber.New()

	app.Get("/", func(c *fiber.Ctx) error {

		rows, err := db.Query("SELECT id, message FROM fortune")

		if err != nil {
			return c.Status(500).SendString(err.Error())
		}

		var fortunes []Fortune

		for rows.Next() {

			var fortune = Fortune{}
			if err := rows.Scan(&fortune.Id, &fortune.Message); err != nil {
				return err
			}

			fortunes = append(fortunes, fortune)
		}

		return c.JSON(fortunes)
	})

	log.Fatal(app.Listen(":5000"))
}
