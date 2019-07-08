using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SensorAPI.Models;

namespace SensorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
       private readonly SensorContext _db;
       public NodesController(SensorContext database)
       {
           _db = database;
           if(_db.Nodes.Count() == 0) {
               _db.Nodes.Add(new Node {
                   Name = "Server",
                   Description = "Default API server node"
               });
               _db.SaveChanges();
           }
       }

       [HttpGet]
       public ActionResult<List<Node>> GetNodes()
       {
           return _db.Nodes.ToList();
       }

       [HttpGet("{id}")]
       public ActionResult<Node> GetNode(long id)
       {
           var node = _db.Nodes.Find(id);
           if(node == null) return NotFound();
           return node;
       }

       [HttpPost]
       public ActionResult<Node> PostNode([FromBody] Node node)
       {
            //var exists = (from x in _db.Nodes where x.Name == node.Name select x.Id).Count();
            var exists = _db.Nodes.Where(o => o.Name == node.Name).Count();
            if(exists == 0) {
                _db.Nodes.Add(new Node {
                    Name = node.Name,
                    Description = node.Description
                });
                _db.SaveChanges();
                return _db.Nodes.Last();
            } else return BadRequest();
       }

        [HttpPut("{id}")]
        public ActionResult<Node> PutNode(long id, [FromBody] Node update)
        {
            var node = _db.Nodes.Find(id);
            if(node == null) return NotFound();
            node.Name = update.Name;
            node.Description = update.Description;
            _db.SaveChanges();
            return node;
        }

        [HttpDelete("{id}")]
        public ActionResult<Node> DeleteNode(long id)
        {
            var node = _db.Nodes.Find(id);
            if(node == null) return NotFound();
            var deleted = new Node {
                Id = 0,
                Name = node.Name,
                Description = node.Description
            };
            _db.Remove(node);
            _db.SaveChanges();
            return deleted;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
       private readonly SensorContext _db;
       public SensorsController(SensorContext database)
       {
           _db = database;
           if(_db.Sensors.Count() == 0) {
               _db.Sensors.Add(new Sensor {
                   NodeId = 1,
                   Type = 1,
                   Name = "CPU Temperature",
                   Description = "Default API server sensor"
               });
               _db.SaveChanges();
           }
       }

       [HttpGet]
       public ActionResult<List<Sensor>> GetSensors()
       {
           return _db.Sensors.ToList();
       }

       [HttpGet("{id}")]
       public ActionResult<Sensor> GetSensor(long id)
       {
           var sensor = _db.Sensors.Find(id);
           if(sensor == null) return NotFound();
           return sensor;
       }

       [HttpPost]
       public ActionResult<Sensor> PostSensor([FromBody] Sensor sensor)
       {
            var exists = _db.Sensors.Where(o => o.Name == sensor.Name).Count();
            if(exists == 0) {
                if(_db.Nodes.Find(sensor.NodeId)==null) return BadRequest();
                _db.Sensors.Add(new Sensor {
                    Name = sensor.Name,
                    NodeId = sensor.NodeId,
                    Type = sensor.Type,
                    Description = sensor.Description
                });
                _db.SaveChanges();
                return _db.Sensors.Last();
            } else return BadRequest();
       }

        [HttpPut("{id}")]
        public ActionResult<Sensor> PutSensor(long id, [FromBody] Sensor update)
        {
            var sensor = _db.Sensors.Find(id);
            if(sensor == null) return NotFound();
            if(_db.Nodes.Find(update.NodeId)==null) return BadRequest();
            sensor.NodeId = update.NodeId;
            sensor.Name = update.Name;
            sensor.Type = update.Type;
            sensor.Description = update.Description;
            _db.SaveChanges();
            return sensor;
        }

        [HttpDelete("{id}")]
        public ActionResult<Sensor> DeleteSensor(long id)
        {
            var sensor = _db.Sensors.Find(id);
            if(sensor == null) return NotFound();
            var deleted = new Sensor {
                SensorId = 0,
                NodeId = sensor.NodeId,
                Type = sensor.Type,
                Name = sensor.Name,
                Description = sensor.Description
            };
            _db.Remove(sensor);
            _db.SaveChanges();
            return deleted;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
       private readonly SensorContext _db;
       public ValuesController(SensorContext database)
       {
           _db = database;
       }

       [HttpGet]
       public ActionResult<List<SensorValue>> GetValues([FromQuery]int limit, [FromQuery]long ts)
       {
            if(limit == 0 || limit > 100) limit = 100;
            if(ts != 0) {
                var values = from x in _db.SensorValues
                where x.Timestamp <= ts
                orderby x.Id descending
                select x;
                return values.Take(limit).ToList();
            } else {
                var values = from x in _db.SensorValues
                orderby x.Id descending
                select x;
                return values.Take(limit).ToList();
            }
       }

       [HttpGet("{id}")]
       public ActionResult<List<SensorValue>> GetSensorValues(long id, [FromQuery]int limit, [FromQuery]long ts)
       {
           if(limit == 0 || limit > 100) limit = 100;
           var sensor = _db.Sensors.Find(id);
           if(sensor == null) return NotFound();
           if(ts != 0) {
               var values = from x in _db.SensorValues
               where x.SensorId == id && x.Timestamp <= ts
               orderby x.Id descending
               select x;
               return values.Take(limit).ToList();
           } else {
               var values = from x in _db.SensorValues
               where x.SensorId == id
               orderby x.Id descending
               select x;
               return values.Take(limit).ToList();
           }
       }

       [HttpPost]
       public void PostValue([FromBody] SensorValue value)
       {
           var sensor = _db.Sensors.Find(value.SensorId);
           if(sensor != null) {
               _db.SensorValues.Add(new SensorValue {
                   SensorId = value.SensorId,
                   Timestamp = value.Timestamp,
                   Value = value.Value
               });
               _db.SaveChanges();
           }
       }
    }

}
