select daily_date, count(1) as cnt from car_daily group by daily_date

select count(1) from taxi_meter where t_start_time >= '2017-08-06 00:00:00' and t_start_time <= '2017-08-06 23:59:59'

select * from taxi_meter where t_start_time >= '2017-08-06 00:00:00' and t_start_time <= '2017-08-06 23:59:59' order by t_imei, t_start_time

select count(1) from taxi_meter group by date(t_start_time)
select date(t_start_time), count(1) from taxi_meter group by date(t_start_time)

delete from car_daily where daily_date = '2017-08-06'
delete from taxi_meter where t_start_time >= '2017-08-06 00:00:00' and t_start_time <= '2017-08-06 23:59:59'
select  count(1) as cnt,sum(t_taxi_fare) , dayofweek(t_start_time) from taxi_meter group by dayofweek(t_start_time)

select 
--update car_daily set car_id_old = car_id;
select count(1) from car

--update car_daily set ceil_income = (CEILING((`car_daily`.`income` / 100)) * 100);

select count(1) as cnt, ceil_income from car_daily where ceil_income >0 group by ceil_income order by ceil_income

SELECT YEAR(daily_date), MONTH(daily_date), count(1) as cnt, sum(income) as income, sum(trip_cnt) as trip_cnt, sum(trip_distance) as trip_distance, sum(distance) as distance 
FROM car_daily 
Where daily_date <= '2017-08-17' and daily_date >= date_add('2017-08-17',INTERVAL -90 day) and income > 0 
Group By  YEAR(daily_date), MONTH(daily_date)
Order By YEAR(daily_date), MONTH(daily_date) asc;

select  count(1) as cnt,sum(t_taxi_fare) as t_taxi_fare, dayofweek(t_start_time) as dayofweek, sum(t_distance) as t_distance From taxi_meter 
    Where t_start_time <= '2017-08-17' and t_start_time >= date_add('2017-08-17',INTERVAL -90 day) 
    Group By dayofweek(t_start_time) order By dayofweek(t_start_time)