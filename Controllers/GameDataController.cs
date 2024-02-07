using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unshaped_gamedata_api.Data;
using unshaped_gamedata_api.Models;

namespace unshaped_gamedata_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameDataController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public GameDataController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/GameData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameData>>> GetGameData()
        {
            return await _context.GameData.ToListAsync();
        }

        // GET: api/GameData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameData>> GetGameData(int id)
        {
            var gameData = await _context.GameData.FindAsync(id);

            if (gameData == null)
            {
                return NotFound();
            }

            return gameData;
        }

        // PUT: api/GameData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameData(int id, GameData gameData)
        {
            if (id != gameData.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GameData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameData>> PostGameData(GameData gameData)
        {
            // Additional Validation
            if (gameData.Hours == null && gameData.Minutes == null) {
                // We need to calculate these values
                var minutes = gameData.TimePlayed % 60;
                var hours = (gameData.TimePlayed - minutes) / 60;

                gameData.Hours = hours;
                gameData.Minutes = minutes;

            }
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameData", new { id = gameData.Id }, gameData);
        }

        // DELETE: api/GameData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameData(int id)
        {
            var gameData = await _context.GameData.FindAsync(id);
            if (gameData == null)
            {
                return NotFound();
            }

            _context.GameData.Remove(gameData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameDataExists(int id)
        {
            return _context.GameData.Any(e => e.Id == id);
        }
    }
}
