/*
 
/////////////////////////
 IQueryable<Speaker> query = _context.Talks
              .Where(t => t.Camp.Moniker == moniker)
              .Select(t => t.Speaker)
              .Where(s => s != null)
              .OrderBy(s => s.LastName)
              .Distinct();

            return await query.ToArrayAsync();



//////////////////////////////

  IQueryable<Talk> query = _context.Talks;

            if (includeSpeakers)
            {
                query = query
                  .Include(t => t.Speaker);
            }

            // Add Query
            query = query
              .Where(t => t.Camp.Moniker == moniker)
              .OrderByDescending(t => t.Title);

            return await query.ToArrayAsync();

///////////////////////////////////


 */