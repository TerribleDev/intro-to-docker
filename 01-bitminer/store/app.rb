require 'sinatra'
require 'date'
require 'json'
set :bind, '0.0.0.0'
set :port, 80
get '/' do
  content_type :json
  getData.to_json
end

get '/store' do
  begin
    requestDate = DateTime.parse(params['dt'])
  rescue
    requestDate = DateTime.now
  end
  hashKey = "#{requestDate.year}-#{requestDate.month}-#{requestDate.day}-#{requestDate.hour}-#{requestDate.minute}-#{requestDate.second}"
  puts hashKey
  coinsInHash = DB.dbHash[hashKey]
  if coinsInHash
    coinsInHash = coinsInHash.next
  else
    puts "setting to zero"
    coinsInHash = 0
  end
  DB.dbHash[hashKey] = coinsInHash
  content_type :json
  getData.to_json
end

def getData
  dt = Time.now
  datumHash = (0...60).to_a.map { |e|
    dts = dt - e
    key = "#{dts.year}-#{dts.month}-#{dts.day}-#{dts.hour}-#{dts.min}-#{dts.sec}"
    data = DB.dbHash[key] || 0
    {date:dts, value: data}
   }
end

class DB
@@DateTimeHash = {}

def self.dbHash
  @@DateTimeHash
end

end
